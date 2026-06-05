# Content type migration request

## What to migrate

- **Source content type**: `TrainingGuides.CatFoodVariant` (Cat food variant) — One of the reusable content types to convert. Represents a cat food product with a specific weight, formulation, and price.
- **Source content type**: `TrainingGuides.CatFood` (Cat food) — One of the reusable content types to convert. Represents cat food parent products, such as a line of cat food from a specific brand.
- **Source content type**: `TrainingGuides.DogCollarVariant` (Dog collar variant) — One of the reusable content types to convert. Represents a dog collar product with a specific color, size, and price.
- **Source content type**: `TrainingGuides.DogCollar` (Dog collar) — One of the reusable content types to convert. Represents a dog collar parent product.
- **Target content type**: `TrainingGuides.GenericProduct` (Generic product) — A consolidated reusable content type that will represent both products and variants.
- **Referencing content type(s)**: 
  - `ProductPage` (Product page) — Web page content type that references reusable products.

## Content type details

### Source content type 1

- Cat food variant

#### C# class

`src/TrainingGuides.Entities/ReusableContentTypes/CatFoodVariant/CatFoodVariant.generated.cs`

#### ClassFormDefinition

```xml
<form>
  <field column="ContentItemDataID" columntype="integer" enabled="true" guid="0ef9ccc9-5320-4fae-a113-c168ff378667" isPK="true" />
  <field column="ContentItemDataCommonDataID" columntype="integer" enabled="true" guid="3e1c75a1-d62b-4f60-b170-4b1081bc3611" refobjtype="cms.contentitemcommondata" reftype="Required" system="true" />
  <field column="ContentItemDataGUID" columntype="guid" enabled="true" guid="e708ccc7-d675-444b-b84a-f3464238b860" isunique="true" system="true" />
  <field allowempty="true" column="CatFoodVariantFormulation" columnprecision="0" columntype="contentitemreference" enabled="true" guid="4ff443bf-b147-4f35-b35d-03d61002f569" visible="true">
    <properties>
      <explanationtextashtml>False</explanationtextashtml>
      <fieldcaption>Formulation</fieldcaption>
      <fielddescriptionashtml>False</fielddescriptionashtml>
    </properties>
    <settings>
      <AllowedContentItemTypeIdentifiers>["a69ed0b4-65d3-459e-847a-2b65e2f82a5a"]</AllowedContentItemTypeIdentifiers>
      <controlname>Kentico.Administration.ContentItemSelector</controlname>
      <SelectionType>contentTypes</SelectionType>
    </settings>
  </field>
  <schema guid="718b3f6f-4d78-4bda-874f-8b54d5d44b7b" name="718b3f6f-4d78-4bda-874f-8b54d5d44b7b"><properties /></schema>
  <schema guid="240eabd8-f0ff-47b8-b27b-b82a8b2801c4" name="240eabd8-f0ff-47b8-b27b-b82a8b2801c4"><properties /></schema>
  <schema guid="d84feead-cb75-44c3-848b-e348f53ea064" name="d84feead-cb75-44c3-848b-e348f53ea064"><properties /></schema>
  <schema guid="00b85aea-91a7-4605-b142-fa38a582dcf1" name="00b85aea-91a7-4605-b142-fa38a582dcf1"><properties /></schema>
  <schema guid="09c6f590-53ee-48f5-9d0e-c572fdd78bc2" name="09c6f590-53ee-48f5-9d0e-c572fdd78bc2"><properties /></schema>
  <schema guid="f2b4c8cb-b9db-41ca-95f4-2e818147cc7d" name="f2b4c8cb-b9db-41ca-95f4-2e818147cc7d"><properties /></schema>
</form>
```
### Source content type 2

- Cat food

#### C# class

`src/TrainingGuides.Entities/ReusableContentTypes/CatFood/CatFood.generated.cs`

#### ClassFormDefinition

```xml
<form>
  <field column="ContentItemDataID" columntype="integer" enabled="true" guid="8f92e728-dd91-4c25-b4d6-8a2788f661d9" isPK="true" />
  <field column="ContentItemDataCommonDataID" columntype="integer" enabled="true" guid="759d6381-3f6a-4d8f-876a-6bca60979a73" refobjtype="cms.contentitemcommondata" reftype="Required" system="true" />
  <field column="ContentItemDataGUID" columntype="guid" enabled="true" guid="c6df4483-2b6b-44f9-a248-7e6b33868c79" isunique="true" system="true" />
  <schema guid="240eabd8-f0ff-47b8-b27b-b82a8b2801c4" name="240eabd8-f0ff-47b8-b27b-b82a8b2801c4"><properties /></schema>
  <schema guid="f8e97d15-2d34-48a9-8f43-903477f81ae0" name="f8e97d15-2d34-48a9-8f43-903477f81ae0"><properties /></schema>
  <field allowempty="true" column="CatFoodVariants" columnprecision="0" columntype="contentitemreference" enabled="true" guid="88a1bd9a-7555-4a98-8fc5-2ee1afc830a9" visible="true">
    <properties>
      <explanationtextashtml>False</explanationtextashtml>
      <fieldcaption>Variants</fieldcaption>
      <fielddescriptionashtml>False</fielddescriptionashtml>
    </properties>
    <settings>
      <AllowedContentItemTypeIdentifiers>["de985d3b-25e6-4204-baef-4903811489e3"]</AllowedContentItemTypeIdentifiers>
      <controlname>Kentico.Administration.ContentItemSelector</controlname>
      <SelectionType>contentTypes</SelectionType>
    </settings>
  </field>
</form>
```

### Source content type 3

- Dog collar variant

#### C# class

`src/TrainingGuides.Entities/ReusableContentTypes/DogCollarVariant/DogCollarVariant.generated.cs`

#### ClassFormDefinition

```xml
<form>
  <field column="ContentItemDataID" columntype="integer" enabled="true" guid="d299e442-7fa7-450d-93a5-4761db15b05c" isPK="true" />
  <field column="ContentItemDataCommonDataID" columntype="integer" enabled="true" guid="84107c5a-af5d-449f-bd06-a3694a164269" refobjtype="cms.contentitemcommondata" reftype="Required" system="true" />
  <field column="ContentItemDataGUID" columntype="guid" enabled="true" guid="21c678e6-b935-4cbd-b7a1-c085e5d33f5a" isunique="true" system="true" />
  <schema guid="240eabd8-f0ff-47b8-b27b-b82a8b2801c4" name="240eabd8-f0ff-47b8-b27b-b82a8b2801c4"><properties /></schema>
  <schema guid="718b3f6f-4d78-4bda-874f-8b54d5d44b7b" name="718b3f6f-4d78-4bda-874f-8b54d5d44b7b"><properties /></schema>
  <schema guid="f2b4c8cb-b9db-41ca-95f4-2e818147cc7d" name="f2b4c8cb-b9db-41ca-95f4-2e818147cc7d"><properties /></schema>
  <schema guid="c0fd2ccf-efbe-4a41-b15f-48eca30539a6" name="c0fd2ccf-efbe-4a41-b15f-48eca30539a6"><properties /></schema>
  <schema guid="8e9bc255-0e70-4034-8c3a-eb349bfe417a" name="8e9bc255-0e70-4034-8c3a-eb349bfe417a"><properties /></schema>
  <schema guid="00b85aea-91a7-4605-b142-fa38a582dcf1" name="00b85aea-91a7-4605-b142-fa38a582dcf1"><properties /></schema>
  <schema guid="09c6f590-53ee-48f5-9d0e-c572fdd78bc2" name="09c6f590-53ee-48f5-9d0e-c572fdd78bc2"><properties /></schema>
</form>
```

### Source content type 4

- Dog collar

#### C# class

`src/TrainingGuides.Entities/ReusableContentTypes/DogCollar/DogCollar.generated.cs`

#### ClassFormDefinition

```xml
<form>
  <field column="ContentItemDataID" columntype="integer" enabled="true" guid="538bbf7f-1cc2-40d0-9f1c-b02be6d688aa" isPK="true" />
  <field column="ContentItemDataCommonDataID" columntype="integer" enabled="true" guid="22ebc340-0f44-47c5-bb41-64f417178ded" refobjtype="cms.contentitemcommondata" reftype="Required" system="true" />
  <field column="ContentItemDataGUID" columntype="guid" enabled="true" guid="f778379d-de57-401c-8a49-0036253231bc" isunique="true" system="true" />
  <schema guid="240eabd8-f0ff-47b8-b27b-b82a8b2801c4" name="240eabd8-f0ff-47b8-b27b-b82a8b2801c4"><properties /></schema>
  <schema guid="849b822b-e92a-4da3-8250-1251bd4fea37" name="849b822b-e92a-4da3-8250-1251bd4fea37"><properties /></schema>
  <schema guid="f8e97d15-2d34-48a9-8f43-903477f81ae0" name="f8e97d15-2d34-48a9-8f43-903477f81ae0"><properties /></schema>
</form>
```

### ContentItemCommonData definition with all schemas

```xml
<form>
  <!-- System fields -->
  <field column="ContentItemCommonDataID" columntype="integer" enabled="true" guid="dcc7d6bf-4c35-4414-9ed7-76f89bf9960d" isPK="true" system="true" />
  <field column="ContentItemCommonDataGUID" columnprecision="0" columntype="guid" enabled="true" guid="8faf5ded-4735-4a11-a8ba-da6e5cfe71b5" system="true" />
  <field column="ContentItemCommonDataContentItemID" columnprecision="0" columntype="integer" enabled="true" guid="91fc5540-0504-4301-a196-d9cc050499f6" refobjtype="cms.contentitem" reftype="Required" system="true">
    <properties><defaultvalue>0</defaultvalue></properties>
  </field>
  <field column="ContentItemCommonDataContentLanguageID" columnprecision="0" columntype="integer" enabled="true" guid="66a47fdf-75d6-4bd6-b99d-f37a816b8676" refobjtype="cms.contentlanguage" reftype="Required" system="true" />
  <field column="ContentItemCommonDataVersionStatus" columnprecision="0" columntype="integer" enabled="true" guid="a26f89d5-5e71-4e55-97e9-0f121c2ec231" system="true" />
  <field column="ContentItemCommonDataIsLatest" columnprecision="0" columntype="boolean" enabled="true" guid="8fda7834-4aeb-4f2d-9ba9-cb3861e56b62" system="true" />
  <field allowempty="true" column="ContentItemCommonDataVisualBuilderWidgets" columnprecision="0" columntype="longtext" enabled="true" guid="b67cbb13-d340-42bf-bb5b-e479bf31f509" system="true" />
  <field allowempty="true" column="ContentItemCommonDataVisualBuilderTemplateConfiguration" columnprecision="0" columntype="longtext" enabled="true" guid="0d5a573b-759a-4dd7-a86b-be2ef2f5bac4" system="true" />
  <field allowempty="true" column="ContentItemCommonDataFirstPublishedWhen" columnprecision="7" columntype="datetime" enabled="true" guid="1318327e-9a4c-47e7-be2e-3da0fe99c15e" system="true" />
  <field allowempty="true" column="ContentItemCommonDataLastPublishedWhen" columnprecision="7" columntype="datetime" enabled="true" guid="169ce320-e362-4b70-b4dd-cc055082c254" system="true" />

  <!-- ArticleSchema (c3b4896f-ba7c-4b75-9cd4-47afa7489ff1) -->
  <schema guid="c3b4896f-ba7c-4b75-9cd4-47afa7489ff1" name="ArticleSchema">...</schema>
  <!-- ArticleSchema fields: ArticleSchemaTitle, ArticleSchemaTeaser, ArticleSchemaSummary, ArticleSchemaText, ArticleSchemaRelatedArticles, ArticleSchemaCategory -->

  <!-- ProductSchema (240eabd8-f0ff-47b8-b27b-b82a8b2801c4) -->
  <schema guid="240eabd8-f0ff-47b8-b27b-b82a8b2801c4" name="ProductSchema">
    <properties><fieldcaption>Product schema</fieldcaption></properties>
  </schema>
  <field column="ProductSchemaName" columnsize="200" columntype="text" guid="19411075-2f29-4e38-93cf-2c63b688c291" ... />
  <field column="ProductSchemaImages" columntype="contentitemreference" guid="84b06b06-1392-422d-82e5-639ed2fa6340" ...>
    <settings>
      <AllowedContentItemTypeIdentifiers>["f77cda0f-5e14-4c20-aae0-6bbb2d9976eb"]</AllowedContentItemTypeIdentifiers>
      <controlname>Kentico.Administration.ContentItemSelector</controlname>
    </settings>
  </field>
  <field column="ProductSchemaDescription" columntype="longtext" guid="2d9a414e-d306-4e28-af27-62ba0666ec92" ... />

  <!-- AmountSchema (d84feead-cb75-44c3-848b-e348f53ea064) -->
  <schema guid="d84feead-cb75-44c3-848b-e348f53ea064" name="AmountSchema">...</schema>
  <!-- Fields: AmountSchemaNumber (decimal), AmountSchemaUnit (dropdown: kg/g/lb/oz/l/ml/etc.) -->

  <!-- ColorPatternSchema (c0fd2ccf-efbe-4a41-b15f-48eca30539a6) -->
  <schema guid="c0fd2ccf-efbe-4a41-b15f-48eca30539a6" name="ColorPatternSchema">...</schema>
  <!-- Field: ColorPattern (taxonomy) -->

  <!-- MaterialSchema (849b822b-e92a-4da3-8250-1251bd4fea37) -->
  <schema guid="849b822b-e92a-4da3-8250-1251bd4fea37" name="MaterialSchema">...</schema>
  <!-- Field: MaterialSchemaMaterial (taxonomy) -->

  <!-- ProductSkuSchema (718b3f6f-4d78-4bda-874f-8b54d5d44b7b) -->
  <schema guid="718b3f6f-4d78-4bda-874f-8b54d5d44b7b" name="ProductSkuSchema">...</schema>
  <field column="ProductSkuSchemaSkuCode" columnsize="200" columntype="text" guid="426c71d2-e0f3-41fa-8b55-50691c0afed8" ... />

  <!-- ProductShippingSchema (09c6f590-53ee-48f5-9d0e-c572fdd78bc2) -->
  <schema guid="09c6f590-53ee-48f5-9d0e-c572fdd78bc2" name="ProductShippingSchema">...</schema>
  <field column="ProductShippingSchemaShippingType" columntype="text" ... />
  <field column="ProductShippingSchemaShippingWeight" columntype="decimal" ... />
  <field column="ProductShippingSchemaWeightUnit" columntype="text" ... />

  <!-- SizeSchema (8e9bc255-0e70-4034-8c3a-eb349bfe417a) -->
  <schema guid="8e9bc255-0e70-4034-8c3a-eb349bfe417a" name="SizeSchema">...</schema>
  <!-- Field: SizeSchemaSize (taxonomy) -->

  <!-- ProductPriceSchema (00b85aea-91a7-4605-b142-fa38a582dcf1) -->
  <schema guid="00b85aea-91a7-4605-b142-fa38a582dcf1" name="ProductPriceSchema">...</schema>
  <field column="ProductPriceSchemaPrice" columntype="decimal" ... />
  <field column="ProductPriceSchemaDiscountCategory" columntype="taxonomy" ... />

  <!-- ProductVariantSchema (f2b4c8cb-b9db-41ca-95f4-2e818147cc7d) -->
  <schema guid="f2b4c8cb-b9db-41ca-95f4-2e818147cc7d" name="ProductVariantSchema">...</schema>
  <field column="ProductVariantSchemaCodeName" columnsize="200" columntype="text" ... />

  <!-- ProductParentSchema (f8e97d15-2d34-48a9-8f43-903477f81ae0) -->
  <schema guid="f8e97d15-2d34-48a9-8f43-903477f81ae0" name="ProductParentSchema">...</schema>
  <field column="ProductParentSchemaVariants" columntype="contentitemreference" ...>
    <settings>
      <AllowedSchemaIdentifiers>["f2b4c8cb-b9db-41ca-95f4-2e818147cc7d"]</AllowedSchemaIdentifiers>
      <SelectionType>reusableFieldSchemas</SelectionType>
    </settings>
  </field>
</form>
```

### Target content type

- Generic product

#### C# class and interface

`src\TrainingGuides.Entities\ReusableContentTypes\GenericProduct\GenericProduct.generated.cs`

#### ClassFormDefinition

```xml
<form>
	<field column="ContentItemDataID" columntype="integer" enabled="true" guid="ed2154ac-df37-4f56-9479-68f97981d905" isPK="true"/>
	<field column="ContentItemDataCommonDataID" columntype="integer" enabled="true" guid="e4996211-9658-4de2-bf19-d2278be4e969" refobjtype="cms.contentitemcommondata" reftype="Required" system="true"/>
	<field column="ContentItemDataGUID" columntype="guid" enabled="true" guid="891f982a-063f-43e4-b75a-410096c43eed" isunique="true" system="true"/>
	<field column="GenericProductName" columnsize="200" columntype="text" enabled="true" guid="a659b80e-1642-45ec-b371-8f3ebb2459ee" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Name</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<controlname>Kentico.Administration.TextInput</controlname>
		</settings>
	</field>
	<field allowempty="true" column="GenericProductDescription" columntype="longtext" enabled="true" guid="fb81839a-d7ea-4071-a9f9-ad4ff3e1ba3c" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Description</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<controlname>Kentico.Administration.TextArea</controlname>
			<MaxRowsNumber>10</MaxRowsNumber>
			<MinRowsNumber>3</MinRowsNumber>
		</settings>
	</field>
	<field allowempty="true" column="GenericProductImages" columnprecision="0" columntype="contentitemreference" enabled="true" guid="9753c77a-14e3-4826-9503-ee9335c39098" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Images</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<AllowedContentItemTypeIdentifiers>["9ecde825-735e-4967-a995-cc97b55adb0f"]</AllowedContentItemTypeIdentifiers>
			<controlname>Kentico.Administration.ContentItemSelector</controlname>
			<SelectionType>contentTypes</SelectionType>
		</settings>
	</field>
	<field allowempty="true" column="ProductSkuCode" columnsize="100" columntype="text" enabled="true" guid="3286a7ac-222f-467f-a1fd-a2145facb727" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>SKU code</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<controlname>Kentico.Administration.TextInput</controlname>
		</settings>
	</field>
	<field allowempty="true" column="GenericProductIsShippingAvailable" columntype="boolean" enabled="true" guid="a1516933-8519-46ce-a979-187916022845" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Is shipping available</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<controlname>Kentico.Administration.Checkbox</controlname>
		</settings>
	</field>
	<field allowempty="true" column="GenericProductShippingWeight" columnprecision="4" columnsize="19" columntype="decimal" enabled="true" guid="25fcab61-4a87-4853-b863-f22997f1fca0" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Shipping weight</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<controlname>Kentico.Administration.DecimalNumberInput</controlname>
		</settings>
	</field>
	<field allowempty="true" column="GenericProductVariants" columnprecision="0" columntype="contentitemreference" enabled="true" guid="49e1366b-fa2c-4efe-85d5-8c262bd4a768" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Variants</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<AllowedContentItemTypeIdentifiers>["4166e17d-3c11-41fd-b0f5-238d2f25d2a9"]</AllowedContentItemTypeIdentifiers>
			<controlname>Kentico.Administration.ContentItemSelector</controlname>
			<SelectionType>contentTypes</SelectionType>
		</settings>
	</field>
	<field allowempty="true" column="GenericProductIsVariant" columntype="boolean" enabled="true" guid="b04c45bb-1588-424b-9383-2e43db863a15" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Is variant</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<controlname>Kentico.Administration.Checkbox</controlname>
		</settings>
	</field>
</form>
```

### Referencing content types

Product page

"This page content type has a `ProductPageProducts` field that 
references *Cat food* and *Dog collar* content types. The migration should update it to reference the newly created *Generic product*
new target type via the `ProductPageGenericProducts` field, and clear the old references."

#### C# class

`src\TrainingGuides.Entities\PageContentTypes\ProductPage\ProductPage.generated.cs`

#### ClassFormDefinition

```xml
<form>
	<field column="ContentItemDataID" columntype="integer" enabled="true" guid="ba52a432-5f06-4355-a1e7-efaa5d02eda9" isPK="true"/>
	<field column="ContentItemDataCommonDataID" columntype="integer" enabled="true" guid="c6bfeca4-e974-498c-8cb8-ce3c7bb5746a" refobjtype="cms.contentitemcommondata" reftype="Required" system="true"/>
	<field column="ContentItemDataGUID" columntype="guid" enabled="true" guid="06231294-8bff-4c98-b9a9-5a90afe23112" isunique="true" system="true"/>
	<field allowempty="true" column="ProductPageProducts" columnprecision="0" columntype="contentitemreference" enabled="true" guid="006815af-b9a1-4191-b865-e729e078d4cf" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Product</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<AllowedSchemaIdentifiers>["240eabd8-f0ff-47b8-b27b-b82a8b2801c4"]</AllowedSchemaIdentifiers>
			<controlname>Kentico.Administration.ContentItemSelector</controlname>
			<MaximumItems>1</MaximumItems>
			<MinimumItems>1</MinimumItems>
			<SelectionType>reusableFieldSchemas</SelectionType>
		</settings>
		<validationrulesdata>
			<ValidationRuleConfiguration>
				<ValidationRuleIdentifier>Kentico.Administration.RequiredValue</ValidationRuleIdentifier>
				<RuleValues/>
			</ValidationRuleConfiguration>
		</validationrulesdata>
	</field>
	<field allowempty="true" column="ProductPageGenericProducts" columnprecision="0" columntype="contentitemreference" enabled="true" guid="d46ba498-daad-403e-bb9b-2b0211ddf434" visible="true">
		<properties>
			<explanationtextashtml>False</explanationtextashtml>
			<fieldcaption>Generic products</fieldcaption>
			<fielddescriptionashtml>False</fielddescriptionashtml>
		</properties>
		<settings>
			<AllowedContentItemTypeIdentifiers>["4166e17d-3c11-41fd-b0f5-238d2f25d2a9"]</AllowedContentItemTypeIdentifiers>
			<controlname>Kentico.Administration.ContentItemSelector</controlname>
			<SelectionType>contentTypes</SelectionType>
		</settings>
	</field>
</form>
```

## Field mapping

<!-- 
  One row per field. Include ALL fields from both source and target.
  - For source fields with no target equivalent, set Target Field to "(drop)".
  - For target fields with no source equivalent, set Source Field to "(none)".
  - Use the Transformation column to describe any conversion logic.
-->

| Source Field | Target Field | Type | Transformation |
|---|---|---|---|
| `ProductSchemaName` | `GenericProductName` | `string` | Direct copy |
| `ProductSchemaImages` | `GenericProductImages` | `IEnumerable<Asset>` | Copy; wrap in ContentItemReference list |
| `ProductSchemaDescription` | `GenericProductDescription` | `string` | Direct copy |
| `ProductParentSchemaVariants` | `GenericProductVariants` | `IEnumerable<GenericProduct>` | Re-map. Reference new `GenericProduct` items corresponding to old `IProductVariantSchema` items as `ContentItemReference` list |
| `ProductSkuSchemaSkuCode` | `ProductSkuCode` | `string` | Direct copy |
| `ProductShippingSchemaShippingType` | `GenericProductIsShippingAvailable` | `bool` | Source field is a text field containing numbers corresponding to different options from a dropdown. Set to `false` if  `ProductShippingSchemaShippingType` contains `0`, `true` otherwise.|
| `ProductShippingSchemaShippingWeight` | `GenericProductShippingWeight` | `decimal` | If the source item's `ProductShippingSchemaWeightUnit` is `lb`, direct copy. if not, convert the value from the specified weight unit to lb. |
| *(none)* | `GenericProductIsVariant ` | `bool` | Set to 'true' for CatFoodVariant and DogCollarVariant items, set to `false` for CatFood and DogCollar items. |


## Additional context

- Process CatFoodVariant and DogCollarVariant items first, so that they are converted before the parent products that reference them.
