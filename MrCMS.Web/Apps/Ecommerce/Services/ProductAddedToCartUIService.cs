using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductAddedToCartUIService : IProductAddedToCartUIService
    {
        private readonly CartModel _cart;
        private readonly IPeopleWhoBoughtThisService _peopleWhoBoughtThisService;
        private readonly IRelatedProductsService _relatedProductsService;
        private readonly IUniquePageService _uniquePageService;

        public ProductAddedToCartUIService(IUniquePageService uniquePageService, CartModel cart,
            IRelatedProductsService relatedProductsService, IPeopleWhoBoughtThisService peopleWhoBoughtThisService)
        {
            _uniquePageService = uniquePageService;
            _cart = cart;
            _relatedProductsService = relatedProductsService;
            _peopleWhoBoughtThisService = peopleWhoBoughtThisService;
        }

        public RedirectResult RedirectToCart()
        {
            return _uniquePageService.RedirectTo<Pages.Cart>();
        }

        public CartModel Cart
        {
            get { return _cart; }
        }

        public RelatedProductsViewModel GetRelatedProducts(ProductVariant productVariant)
        {
            return _relatedProductsService.GetRelatedProducts(productVariant);
        }

        public PeopleWhoBoughtThisAlsoBoughtViewModel CustomersAlsoBought(ProductVariant productVariant)
        {
            return _peopleWhoBoughtThisService.GetAlsoBoughtViewModel(productVariant);
        }
    }
}