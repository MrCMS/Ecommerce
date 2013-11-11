using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductAddedToCartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;

        public ProductAddedToCartController(CartModel cartModel)
        {
            _cartModel = cartModel;
        }

        public ViewResult Show(ProductAddedToCart page, ProductVariant productVariant)
        {
            ViewData["productvariant"] = productVariant;

            return View(page);
        }

        public PartialViewResult ItemDetails(ProductVariant productVariant)
        {
            return PartialView(productVariant);
        }

        public PartialViewResult CartDetails()
        {
            return PartialView(_cartModel);
        }
    }
}