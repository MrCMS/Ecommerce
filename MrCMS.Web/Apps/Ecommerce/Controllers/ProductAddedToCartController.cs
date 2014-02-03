using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics.Products;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductAddedToCartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;
        private readonly IDocumentService _documentService;
        private readonly CartModel _cart;

        public ProductAddedToCartController(CartModel cartModel, IDocumentService documentService, CartModel cart)
        {
            _cartModel = cartModel;
            _documentService = documentService;
            _cart = cart;
        }

        public ActionResult Show(ProductAddedToCart page, ProductVariant productVariant, int quantity = 1)
        {
            if (productVariant == null || productVariant.Id == 0) //model binder always creates a product variant so shouldn't be null
                return _documentService.RedirectTo<Cart>();
            ViewData["productvariant"] = productVariant;
            ViewData["quantity"] = quantity;
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

        public PartialViewResult RelatedProducts(ProductVariant productVariant)
        {
            var model = new RelatedProductsViewModel
                {
                    Title = "Related products",
                    Products = new List<Product>(),
                    Cart = _cart
                };
            var products = new List<Product>();
            if (productVariant.Product.Categories.Any())
                products.AddRange(productVariant.Product.Categories.First().Products.Where(x => x.Id != productVariant.Product.Id && x.Published).Take(4));
            model.Products = products.Distinct().ToList();
            return PartialView(model);
        }

        public PartialViewResult CustomersAlsoBought(ProductVariant productvariant)
        {
            var model = new PeopleWhoBoughtThisAlsoBoughtViewModel
                {
                    Title = "Customers also bought",
                    Products = new List<Product>()
                };
            model.Products =
                MrCMSApplication.Get<IProductAnalyticsService>()
                                .GetListOfProductsWhoWhereAlsoBought(productvariant.Product);
            return PartialView(model);
        }
    }
}