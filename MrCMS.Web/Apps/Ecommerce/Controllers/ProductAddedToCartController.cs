using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductAddedToCartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductAddedToCartUIService _productAddedToCartUIService;

        public ProductAddedToCartController(IProductAddedToCartUIService productAddedToCartUIService)
        {
            _productAddedToCartUIService = productAddedToCartUIService;
        }

        public ActionResult Show(ProductAddedToCart page, ProductVariant productVariant, int quantity = 1)
        {
            if (productVariant == null || productVariant.Id == 0)
                return _productAddedToCartUIService.RedirectToCart();

            ViewData["productvariant"] = productVariant;
            ViewData["quantity"] = quantity;
            ViewData["cart"] = _productAddedToCartUIService.Cart;
            return View(page);
        }

        public PartialViewResult ItemDetails(ProductVariant productVariant)
        {
            return PartialView(productVariant);
        }

        public PartialViewResult CartDetails()
        {
            return PartialView(_productAddedToCartUIService.Cart);
        }

        public PartialViewResult RelatedProducts(ProductVariant productVariant)
        {
            return PartialView(_productAddedToCartUIService.GetRelatedProducts(productVariant));
        }

        public PartialViewResult CustomersAlsoBought(ProductVariant productvariant)
        {
            return PartialView(_productAddedToCartUIService.CustomersAlsoBought(productvariant));
        }
    }
}