using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using System;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductVariantController : MrCMSAppUIController<EcommerceApp>
    {
        [HttpGet]
        public JsonResult GetPriceBreaksForProductVariant(ProductVariant productVariant)
        {
            return productVariant != null
                       ? Json(
                           productVariant.PriceBreaks.Select(priceBreak => new { priceBreak.Quantity, priceBreak.Price }),
                           JsonRequestBehavior.AllowGet)
                       : Json(String.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}