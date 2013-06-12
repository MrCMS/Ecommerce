using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using System;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductVariantController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;

        public ProductVariantController(IProductVariantService productVariantService, ITaxRateManager taxRateManager)
        {
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
        }
        
        [HttpGet]
        public JsonResult GetPriceBreaksForProductVariant(int productVariantId = 0)
        {
            if (productVariantId != 0)
                return Json(_productVariantService.GetAllPriceBreaksForProductVariant(productVariantId).Select(priceBreak => new { Quantity = priceBreak.Quantity, Price = priceBreak.Price }),JsonRequestBehavior.AllowGet);

            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}