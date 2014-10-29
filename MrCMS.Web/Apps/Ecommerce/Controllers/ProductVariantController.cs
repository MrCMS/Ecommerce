using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using System;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductVariantController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IProductVariantUIService _productVariantUIService;

        public ProductVariantController(CartModel cart, IProductVariantUIService productVariantUIService)
        {
            _cart = cart;
            _productVariantUIService = productVariantUIService;
        }

        [HttpGet]
        public JsonResult GetPriceBreaksForProductVariant(ProductVariant productVariant)
        {
            return productVariant != null
                       ? Json(
                           productVariant.PriceBreaks.Select(priceBreak => new { priceBreak.Quantity, priceBreak.Price }),
                           JsonRequestBehavior.AllowGet)
                       : Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Details(ProductVariant productVariant)
        {
            ViewData["cart"] = _cart;
            ViewData["can-buy-status"] = _productVariantUIService.CanBuyAny(productVariant);
            return PartialView(productVariant);
        }
    }
}