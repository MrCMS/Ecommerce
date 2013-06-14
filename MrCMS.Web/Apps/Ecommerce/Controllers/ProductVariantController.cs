using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using System;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductVariantController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public JsonResult GetPriceBreaksForProductVariant(int? productVariantId)
        {
            return productVariantId.HasValue
                       ? Json(
                           _productVariantService.GetAllPriceBreaksForProductVariant(productVariantId.Value)
                                                 .Select(priceBreak => new { priceBreak.Quantity, priceBreak.Price }),
                           JsonRequestBehavior.AllowGet)
                       : Json(String.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}