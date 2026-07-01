using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Membership;
using CMS.Websites;
using CMS.Websites.Routing;
using CMS.Workspaces;

namespace TrainingGuides.Web.OneTimeCode;

public class ProductConsolidationConverter
{
    private readonly IWebsiteChannelContext websiteChannelContext;
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IInfoProvider<WorkspaceInfo> workspaceInfoProvider;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;

    private readonly IWebPageManager webPageManager;
    private readonly IContentItemManager contentItemManager;

    private const string ADMINISTRATOR_USERNAME = "administrator";
    private const string DEFAULT_WORKSPACE_NAME = "KenticoDefault";
    private const string NO_SHIPPING_VALUE = "0";
    private const string WEIGHT_UNIT_LB = "lb";
    private const string WEIGHT_UNIT_KG = "kg";
    private const string WEIGHT_UNIT_G = "g";
    private const string WEIGHT_UNIT_OZ = "oz";

    private const decimal LB_PER_KG = 2.20462m;
    private const decimal LB_PER_G = 0.00220462m;
    private const decimal LB_PER_OZ = 0.0625m;

    /// <summary>
    /// Maps old ContentItemID → new GenericProduct ContentItemID for all converted items.
    /// </summary>
    private readonly Dictionary<int, int> oldToNewIdMap = [];

    public ProductConsolidationConverter(
        IInfoProvider<UserInfo> userInfoProvider,
        IContentItemManagerFactory contentItemManagerFactory,
        IWebsiteChannelContext websiteChannelContext,
        IWebPageManagerFactory webPageManagerFactory,
        IContentQueryExecutor contentQueryExecutor,
        IInfoProvider<WorkspaceInfo> workspaceInfoProvider,
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider)
    {
        this.websiteChannelContext = websiteChannelContext;
        this.contentQueryExecutor = contentQueryExecutor;
        this.workspaceInfoProvider = workspaceInfoProvider;
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;

        var adminUser = userInfoProvider.Get()
            .WhereEquals(nameof(UserInfo.UserName), ADMINISTRATOR_USERNAME)
            .FirstOrDefault();

        if (adminUser is null)
        {
            throw new Exception("Administrator user not found. Cannot proceed with product consolidation migration.");
        }

        contentItemManager = contentItemManagerFactory.Create(adminUser.UserID);
        webPageManager = webPageManagerFactory.Create(websiteChannelContext.WebsiteChannelID, adminUser.UserID);
    }

    /// <summary>
    /// Main method to convert old product content types to GenericProduct and update ProductPage references.
    /// </summary>
    public async Task<List<ProductConversionAttempt>> Convert()
    {
        List<ProductConversionAttempt> attempts = [];

        // Step 1: Convert variant items first (so parent products can reference them)
        var catFoodVariants = await RetrieveItems<CatFoodVariant>(CatFoodVariant.CONTENT_TYPE_NAME);
        attempts.AddRange(await ConvertVariantItems(catFoodVariants, CatFoodVariant.CONTENT_TYPE_NAME));

        var dogCollarVariants = await RetrieveItems<DogCollarVariant>(DogCollarVariant.CONTENT_TYPE_NAME);
        attempts.AddRange(await ConvertVariantItems(dogCollarVariants, DogCollarVariant.CONTENT_TYPE_NAME));

        // Step 2: Convert parent products (after variants, so variant references can be resolved)
        var catFoods = await RetrieveItems<CatFood>(CatFood.CONTENT_TYPE_NAME, linkedItemsLevel: 2);
        attempts.AddRange(await ConvertParentItems(catFoods, CatFood.CONTENT_TYPE_NAME));

        var dogCollars = await RetrieveItems<DogCollar>(DogCollar.CONTENT_TYPE_NAME, linkedItemsLevel: 2);
        attempts.AddRange(await ConvertParentItems(dogCollars, DogCollar.CONTENT_TYPE_NAME));

        // Step 3: Update ProductPage references
        await UpdateProductPageReferences(attempts);

        return attempts;
    }

    #region Retrieval

    private async Task<IEnumerable<T>> RetrieveItems<T>(string contentTypeName, int linkedItemsLevel = 1)
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentType(contentTypeName, config => config.WithLinkedItems(linkedItemsLevel));

        var queryOptions = new ContentQueryExecutionOptions { ForPreview = false };

        return await contentQueryExecutor.GetMappedResult<T>(builder, queryOptions);
    }

    private async Task<IEnumerable<ProductPage>> RetrieveProductPagesLinkingProduct(int oldProductId)
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentType(
                ProductPage.CONTENT_TYPE_NAME,
                config => config
                    .Linking(nameof(ProductPage.ProductPageProducts), [oldProductId])
                    .ForWebsite(websiteChannelContext.WebsiteChannelName)
                    .WithLinkedItems(2));

        var queryOptions = new ContentQueryExecutionOptions { ForPreview = false };

        return await contentQueryExecutor.GetMappedResult<ProductPage>(builder, queryOptions);
    }

    #endregion

    #region Helpers

    private string GetContentLanguageName(int contentLanguageId)
    {
        var contentLanguage = contentLanguageInfoProvider.Get(contentLanguageId);
        return contentLanguage?.ContentLanguageName ?? string.Empty;
    }

    private async Task<string> GetWorkspaceName(int contentItemId)
    {
        var metadata = await contentItemManager.GetContentItemMetadata(contentItemId);

        string result = await workspaceInfoProvider.Get()
            .WhereEquals(nameof(WorkspaceInfo.WorkspaceID), metadata.WorkspaceId)
            .AsSingleColumn(nameof(WorkspaceInfo.WorkspaceName))
            .GetScalarResultAsync<string>();

        return result ?? DEFAULT_WORKSPACE_NAME;
    }

    private static string GetNewProductCodeName(IContentItemFieldsSource oldItem) =>
        $"{oldItem.SystemFields.ContentItemName}-generic";

    private async Task<string> GetDisplayName(
        IContentItemFieldsSource oldItem,
        string languageName,
        ProductConversionAttempt attempt)
    {
        try
        {
            return (await contentItemManager.GetContentItemLanguageMetadata(
                oldItem.SystemFields.ContentItemID, languageName)).DisplayName;
        }
        catch (Exception ex)
        {
            string fallback = oldItem is IProductSchema product
                ? product.ProductSchemaName
                : oldItem.SystemFields.ContentItemName;

            attempt.Exceptions.Add(new Exception(
                $"Failed to retrieve display name for [{fallback}] ID [{oldItem.SystemFields.ContentItemID}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty)));

            return fallback;
        }
    }

    private static bool ConvertShippingType(string shippingType) =>
        !string.IsNullOrWhiteSpace(shippingType) && shippingType != NO_SHIPPING_VALUE;

    private static decimal ConvertWeightToLb(decimal weight, string weightUnit)
    {
        if (string.IsNullOrWhiteSpace(weightUnit))
            return weight;

        return weightUnit.ToLowerInvariant() switch
        {
            WEIGHT_UNIT_LB => weight,
            WEIGHT_UNIT_KG => weight * LB_PER_KG,
            WEIGHT_UNIT_G => weight * LB_PER_G,
            WEIGHT_UNIT_OZ => weight * LB_PER_OZ,
            _ => weight,
        };
    }

    private static List<ContentItemReference> BuildImageReferences(IEnumerable<ProductImage> images)
    {
        if (images is null)
        {
            return [];
        }

        return images
            .Where(img => img?.SystemFields is not null && img.SystemFields.ContentItemGUID != Guid.Empty)
            .Select(img => new ContentItemReference { Identifier = img.SystemFields.ContentItemGUID })
            .ToList();
    }

    private async Task<Guid?> GetNewProductGuid(int? newProductId)
    {
        if (newProductId is null or <= 0)
        {
            return null;
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentType(GenericProduct.CONTENT_TYPE_NAME, config => config
                .Where(w => w.WhereEquals(
                    nameof(GenericProduct.SystemFields.ContentItemID), newProductId.Value))
                .TopN(1));

        var queryOptions = new ContentQueryExecutionOptions { ForPreview = false };
        var results = await contentQueryExecutor.GetMappedResult<GenericProduct>(builder, queryOptions);

        return results.FirstOrDefault()?.SystemFields.ContentItemGUID;
    }

    #endregion

    #region Variant conversion

    private static ContentItemData BuildVariantContentItemData(
        IProductSchema product,
        IProductSkuSchema sku,
        IProductShippingSchema shipping)
    {
        return new ContentItemData(new Dictionary<string, object>
        {
            { nameof(GenericProduct.GenericProductName), product.ProductSchemaName ?? string.Empty },
            { nameof(GenericProduct.GenericProductDescription), product.ProductSchemaDescription ?? string.Empty },
            { nameof(GenericProduct.GenericProductImages), BuildImageReferences(product.ProductSchemaImages) },
            { nameof(GenericProduct.ProductSkuCode), sku.ProductSkuSchemaSkuCode ?? string.Empty },
            { nameof(GenericProduct.GenericProductIsShippingAvailable), ConvertShippingType(shipping.ProductShippingSchemaShippingType) },
            { nameof(GenericProduct.GenericProductShippingWeight), ConvertWeightToLb(shipping.ProductShippingSchemaShippingWeight, shipping.ProductShippingSchemaWeightUnit) },
            { nameof(GenericProduct.GenericProductVariants), new List<ContentItemReference>() },
            { nameof(GenericProduct.GenericProductIsVariant), true },
        });
    }

    private async Task<List<ProductConversionAttempt>> ConvertVariantItems<T>(
        IEnumerable<T> items,
        string sourceTypeName)
        where T : IContentItemFieldsSource, IProductSchema, IProductSkuSchema, IProductShippingSchema
    {
        List<ProductConversionAttempt> attempts = [];

        foreach (var item in items)
        {
            string languageName = GetContentLanguageName(item.SystemFields.ContentItemCommonDataContentLanguageID);
            int oldId = item.SystemFields.ContentItemID;

            var previousAttempts = attempts.Where(a => a.OldItemContentItemId == oldId);
            var previousWithValidId = previousAttempts.Where(a => a.NewProductContentItemId is not null and > 0);

            ProductConversionAttempt currentAttempt;

            if (!previousWithValidId.Any())
            {
                currentAttempt = previousAttempts.FirstOrDefault()
                    ?? new ProductConversionAttempt(
                        oldItemName: item.ProductSchemaName ?? item.SystemFields.ContentItemName,
                        oldItemContentItemId: oldId,
                        sourceContentTypeName: sourceTypeName,
                        isVariant: true);

                var contentItemData = BuildVariantContentItemData(item, item, item);
                int newId = await CreateNewProduct(item, languageName, contentItemData, currentAttempt);
                currentAttempt.NewProductContentItemId = newId > 0 ? newId : null;

                if (newId > 0)
                {
                    oldToNewIdMap[oldId] = newId;
                }

                if (!attempts.Contains(currentAttempt))
                {
                    attempts.Add(currentAttempt);
                }
            }
            else
            {
                currentAttempt = previousWithValidId.First();
                int newId = currentAttempt.NewProductContentItemId ?? 0;

                if (currentAttempt.FinishedLanguagesReusable.Contains(languageName))
                {
                    currentAttempt.Exceptions.Add(new Exception(
                        $"Skipped duplicate language version [{languageName}] for [{item.ProductSchemaName}] ID [{oldId}]."));
                }
                else
                {
                    var contentItemData = BuildVariantContentItemData(item, item, item);
                    await AddLanguageVersion(item, newId, languageName, contentItemData, currentAttempt);
                }
            }

            if (currentAttempt.NewProductContentItemId is > 0)
            {
                await PublishItem(item, currentAttempt.NewProductContentItemId.Value, languageName, currentAttempt);
            }
        }

        return attempts;
    }

    #endregion

    #region Parent conversion

    private async Task<ContentItemData> BuildParentContentItemData(
        IProductSchema product,
        IProductParentSchema parent,
        ProductConversionAttempt attempt)
    {
        var variantRefs = new List<ContentItemReference>();

        if (parent.ProductParentSchemaVariants is not null)
        {
            foreach (var variant in parent.ProductParentSchemaVariants)
            {
                if (variant is IContentItemFieldsSource variantItem)
                {
                    int oldVariantId = variantItem.SystemFields.ContentItemID;

                    if (oldToNewIdMap.TryGetValue(oldVariantId, out int newVariantId))
                    {
                        var guid = await GetNewProductGuid(newVariantId);
                        if (guid is Guid newGuid)
                        {
                            if (!variantRefs.Any(r => r.Identifier == newGuid))
                            {
                                variantRefs.Add(new ContentItemReference { Identifier = newGuid });
                            }
                        }
                        else
                        {
                            attempt.Exceptions.Add(new Exception(
                                $"Could not resolve GUID for new variant ID [{newVariantId}] (old ID [{oldVariantId}])."));
                        }
                    }
                    else
                    {
                        attempt.Exceptions.Add(new Exception(
                            $"Old variant ID [{oldVariantId}] not found in conversion map. Was it converted?"));
                    }
                }
                else
                {
                    attempt.Exceptions.Add(new Exception(
                        $"Variant of type [{variant?.GetType().Name}] does not implement {nameof(IContentItemFieldsSource)}."));
                }
            }
        }

        return new ContentItemData(new Dictionary<string, object>
        {
            { nameof(GenericProduct.GenericProductName), product.ProductSchemaName ?? string.Empty },
            { nameof(GenericProduct.GenericProductDescription), product.ProductSchemaDescription ?? string.Empty },
            { nameof(GenericProduct.GenericProductImages), BuildImageReferences(product.ProductSchemaImages) },
            { nameof(GenericProduct.ProductSkuCode), string.Empty },
            { nameof(GenericProduct.GenericProductIsShippingAvailable), false },
            { nameof(GenericProduct.GenericProductShippingWeight), 0m },
            { nameof(GenericProduct.GenericProductVariants), variantRefs },
            { nameof(GenericProduct.GenericProductIsVariant), false },
        });
    }

    private async Task<List<ProductConversionAttempt>> ConvertParentItems<T>(
        IEnumerable<T> items,
        string sourceTypeName)
        where T : IContentItemFieldsSource, IProductSchema, IProductParentSchema
    {
        List<ProductConversionAttempt> attempts = [];

        foreach (var item in items)
        {
            string languageName = GetContentLanguageName(item.SystemFields.ContentItemCommonDataContentLanguageID);
            int oldId = item.SystemFields.ContentItemID;

            var previousAttempts = attempts.Where(a => a.OldItemContentItemId == oldId);
            var previousWithValidId = previousAttempts.Where(a => a.NewProductContentItemId is not null and > 0);

            ProductConversionAttempt currentAttempt;

            if (!previousWithValidId.Any())
            {
                currentAttempt = previousAttempts.FirstOrDefault()
                    ?? new ProductConversionAttempt(
                        oldItemName: item.ProductSchemaName ?? item.SystemFields.ContentItemName,
                        oldItemContentItemId: oldId,
                        sourceContentTypeName: sourceTypeName,
                        isVariant: false);

                var contentItemData = await BuildParentContentItemData(item, item, currentAttempt);
                int newId = await CreateNewProduct(item, languageName, contentItemData, currentAttempt);
                currentAttempt.NewProductContentItemId = newId > 0 ? newId : null;

                if (newId > 0)
                {
                    oldToNewIdMap[oldId] = newId;
                }

                if (!attempts.Contains(currentAttempt))
                {
                    attempts.Add(currentAttempt);
                }
            }
            else
            {
                currentAttempt = previousWithValidId.First();
                int newId = currentAttempt.NewProductContentItemId ?? 0;

                if (currentAttempt.FinishedLanguagesReusable.Contains(languageName))
                {
                    currentAttempt.Exceptions.Add(new Exception(
                        $"Skipped duplicate language version [{languageName}] for [{item.ProductSchemaName}] ID [{oldId}]."));
                }
                else
                {
                    var contentItemData = await BuildParentContentItemData(item, item, currentAttempt);
                    await AddLanguageVersion(item, newId, languageName, contentItemData, currentAttempt);
                }
            }

            if (currentAttempt.NewProductContentItemId is > 0)
            {
                await PublishItem(item, currentAttempt.NewProductContentItemId.Value, languageName, currentAttempt);
            }
        }

        return attempts;
    }

    #endregion

    #region Create / Language variant / Publish operations

    private async Task<int> CreateNewProduct(
        IContentItemFieldsSource oldItem,
        string languageName,
        ContentItemData contentItemData,
        ProductConversionAttempt attempt)
    {
        string displayName = await GetDisplayName(oldItem, languageName, attempt);

        var createParams = new CreateContentItemParameters(
            contentTypeName: GenericProduct.CONTENT_TYPE_NAME,
            name: GetNewProductCodeName(oldItem),
            displayName: displayName,
            languageName: languageName,
            workspaceName: await GetWorkspaceName(oldItem.SystemFields.ContentItemID));

        try
        {
            int newId = await contentItemManager.Create(createParams, contentItemData);

            if (newId > 0)
            {
                attempt.LogMessages.Add(
                    $"Created GenericProduct for [{attempt.OldItemName}] with new ID [{newId}] in language [{languageName}].");
                attempt.FinishedLanguagesReusable.Add(languageName);
            }
            else
            {
                throw new Exception($"Invalid ID value [{newId}].");
            }

            return newId;
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to create GenericProduct for [{attempt.OldItemName}] ID [{oldItem.SystemFields.ContentItemID}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
            return -1;
        }
    }

    private async Task AddLanguageVersion(
        IContentItemFieldsSource oldItem,
        int newItemId,
        string languageName,
        ContentItemData contentItemData,
        ProductConversionAttempt attempt)
    {
        string displayName = await GetDisplayName(oldItem, languageName, attempt);

        var languageVariantParams = new CMS.ContentEngine.CreateLanguageVariantParameters(
            newItemId, displayName, languageName);

        try
        {
            if (await contentItemManager.TryCreateLanguageVariant(languageVariantParams, contentItemData))
            {
                attempt.LogMessages.Add(
                    $"Added language version [{languageName}] for [{attempt.OldItemName}] ID [{newItemId}].");
                attempt.FinishedLanguagesReusable.Add(languageName);
            }
            else
            {
                throw new Exception("TryCreateLanguageVariant returned false.");
            }
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to add language version [{languageName}] for [{attempt.OldItemName}] ID [{newItemId}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    private async Task PublishItem(
        IContentItemFieldsSource oldItem,
        int newItemId,
        string languageName,
        ProductConversionAttempt attempt)
    {
        try
        {
            if (await contentItemManager.TryPublish(newItemId, languageName))
            {
                var unpublishDate = await GetReusableUnpublishDateIfScheduled(oldItem, languageName, attempt);
                await TryScheduleReusableUnpublish(unpublishDate, oldItem, newItemId, languageName, attempt);
            }
            else
            {
                throw new Exception("TryPublish returned false.");
            }
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to publish [{attempt.OldItemName}] ID [{newItemId}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    private async Task<DateTime?> GetReusableUnpublishDateIfScheduled(
        IContentItemFieldsSource item,
        string languageName,
        ProductConversionAttempt attempt)
    {
        try
        {
            if (await contentItemManager.IsUnpublishScheduled(item.SystemFields.ContentItemID, languageName))
            {
                return (await contentItemManager.GetContentItemLanguageMetadata(
                    item.SystemFields.ContentItemID, languageName)).ScheduledUnpublishWhen;
            }
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to check unpublish schedule for [{item.SystemFields.ContentItemName}] ID [{item.SystemFields.ContentItemID}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }

        return null;
    }

    private async Task TryScheduleReusableUnpublish(
        DateTime? unpublishDate,
        IContentItemFieldsSource oldItem,
        int newItemId,
        string languageName,
        ProductConversionAttempt attempt)
    {
        try
        {
            if (unpublishDate is DateTime unpDate && unpDate > DateTime.MinValue)
            {
                await contentItemManager.ScheduleUnpublish(newItemId, languageName, unpDate);
                attempt.LogMessages.Add(
                    $"Scheduled unpublish for [{oldItem.SystemFields.ContentItemName}] ID [{newItemId}] language [{languageName}] at [{unpublishDate}].");
            }
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to schedule unpublish for ID [{newItemId}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    #endregion

    #region ProductPage reference updates

    private async Task UpdateProductPageReferences(List<ProductConversionAttempt> attempts)
    {
        // Update pages for all converted items (parent products primarily, but also variants if referenced by pages)
        foreach (var attempt in attempts.Where(a => a.NewProductContentItemId is > 0))
        {
            var pages = await RetrieveProductPagesLinkingProduct(attempt.OldItemContentItemId);

            foreach (var page in pages)
            {
                string languageName = GetContentLanguageName(page.SystemFields.ContentItemCommonDataContentLanguageID);
                await CreateAndUpdatePageDraft(page, languageName, attempt);
            }
        }
    }

    private async Task CreateAndUpdatePageDraft(
        ProductPage page,
        string languageName,
        ProductConversionAttempt attempt)
    {
        var unpublishDate = await GetPageUnpublishDateIfScheduled(page, languageName, attempt);

        bool newCreated = false;

        try
        {
            newCreated = await webPageManager.TryCreateDraft(page.SystemFields.WebPageItemID, languageName);
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to create draft for page [{page.SystemFields.WebPageItemTreePath}] ID [{page.SystemFields.ContentItemID}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }

        bool updated = await UpdatePageDraft(page, languageName, attempt, unpublishDate);

        if (!newCreated && !updated)
        {
            attempt.Exceptions.Add(new Exception(
                $"Could not create or update draft for page [{page.SystemFields.WebPageItemTreePath}] ID [{page.SystemFields.ContentItemID}] language [{languageName}]."));
        }
    }

    private async Task<bool> UpdatePageDraft(
        ProductPage page,
        string languageName,
        ProductConversionAttempt attempt,
        DateTime? unpublishDate)
    {
        var newProductGuid = await GetNewProductGuid(attempt.NewProductContentItemId);

        if (newProductGuid is not Guid newGuid)
        {
            attempt.Exceptions.Add(new Exception(
                $"Could not find new GenericProduct with ID [{attempt.NewProductContentItemId}] to update page [{page.SystemFields.WebPageItemTreePath}]."));
            return false;
        }

        var contentItemData = new ContentItemData(new Dictionary<string, object>
        {
            { nameof(ProductPage.ProductPageProducts), new List<ContentItemReference>() },
            { nameof(ProductPage.ProductPageGenericProducts), new List<ContentItemReference>
                {
                    new() { Identifier = newGuid }
                }
            },
        });

        try
        {
            if (await webPageManager.TryUpdateDraft(page.SystemFields.WebPageItemID, languageName, new UpdateDraftData(contentItemData)))
            {
                if (await webPageManager.TryPublish(page.SystemFields.WebPageItemID, languageName))
                {
                    attempt.LogMessages.Add(
                        $"Updated and published page [{page.SystemFields.WebPageItemTreePath}] language [{languageName}] with new GenericProduct reference.");

                    await TrySchedulePageUnpublish(unpublishDate, page, languageName, attempt);
                    return true;
                }

                attempt.Exceptions.Add(new Exception(
                    $"Updated draft but failed to publish page [{page.SystemFields.WebPageItemTreePath}] language [{languageName}]."));
                return false;
            }
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to update page draft [{page.SystemFields.WebPageItemTreePath}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
            return false;
        }

        attempt.Exceptions.Add(new Exception(
            $"Could not update draft for page [{page.SystemFields.WebPageItemTreePath}] language [{languageName}]."));
        return false;
    }

    private async Task<DateTime?> GetPageUnpublishDateIfScheduled(
        ProductPage page,
        string languageName,
        ProductConversionAttempt attempt)
    {
        try
        {
            if (await webPageManager.IsUnpublishScheduled(page.SystemFields.WebPageItemID, languageName))
            {
                return (await webPageManager.GetContentItemLanguageMetadata(
                    page.SystemFields.WebPageItemID, languageName)).ScheduledUnpublishWhen;
            }
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to check unpublish schedule for page [{page.SystemFields.WebPageItemTreePath}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }

        return null;
    }

    private async Task TrySchedulePageUnpublish(
        DateTime? unpublishDate,
        ProductPage page,
        string languageName,
        ProductConversionAttempt attempt)
    {
        try
        {
            if (unpublishDate is DateTime unpDate && unpDate > DateTime.MinValue)
            {
                await webPageManager.ScheduleUnpublish(page.SystemFields.WebPageItemID, languageName, unpDate);
                attempt.LogMessages.Add(
                    $"Scheduled unpublish for page [{page.SystemFields.WebPageItemTreePath}] language [{languageName}] at [{unpublishDate}].");
            }
        }
        catch (Exception ex)
        {
            string newMessage = $"Failed to schedule unpublish for page [{page.SystemFields.WebPageItemTreePath}] language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    #endregion
}

public class ProductConversionAttempt
{
    public string OldItemName { get; set; } = string.Empty;
    public int OldItemContentItemId { get; set; }
    public string SourceContentTypeName { get; set; } = string.Empty;
    public int? NewProductContentItemId { get; set; }
    public bool IsVariant { get; set; }
    public List<Exception> Exceptions { get; set; } = [];
    public List<string> LogMessages { get; set; } = [];
    public List<string> FinishedLanguagesReusable { get; set; } = [];
    public List<string> FinishedLanguagesPage { get; set; } = [];

    public ProductConversionAttempt(
        string oldItemName,
        int oldItemContentItemId,
        string sourceContentTypeName,
        bool isVariant,
        int? newProductContentItemId = null)
    {
        OldItemName = oldItemName;
        OldItemContentItemId = oldItemContentItemId;
        SourceContentTypeName = sourceContentTypeName;
        IsVariant = isVariant;
        NewProductContentItemId = newProductContentItemId;
    }
}
