using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class PeopleWhoBoughtThisService : IPeopleWhoBoughtThisService
    {
        private readonly IProductAnalyticsService _productAnalyticsService;
        private readonly CartModel _cart;

        public PeopleWhoBoughtThisService(IProductAnalyticsService productAnalyticsService, CartModel cart)
        {
            _productAnalyticsService = productAnalyticsService;
            _cart = cart;
        }

        public PeopleWhoBoughtThisAlsoBoughtViewModel GetAlsoBoughtViewModel(Product product)
        {
            var model = new PeopleWhoBoughtThisAlsoBoughtViewModel { Title = string.Empty };
            if (product != null)
            {
                model.Title = string.Format("People Who Bought '{0}' Also Bought", product.Name);
                model.Products = _productAnalyticsService.GetListOfProductsWhoWhereAlsoBought(product);
            }
            model.Cart = _cart;
            return model;
        }

        public PeopleWhoBoughtThisAlsoBoughtViewModel GetAlsoBoughtViewModel(ProductVariant variant)
        {
            return variant == null
                ? GetAlsoBoughtViewModel((Product)null)
                : GetAlsoBoughtViewModel(variant.Product);
        }
    }
}