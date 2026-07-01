using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.OneTimeCode;

public class ConvertProductsController(ProductConsolidationConverter converter) : Controller
{
    [HttpGet("/ConvertProducts")]
    public async Task<IActionResult> Convert()
    {
        var attempts = await converter.Convert();
        return View("~/OneTimeCode/ConvertProductsView.cshtml", attempts);
    }
}
