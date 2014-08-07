using System;
using System.Collections.Generic;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics.Products;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetPeopleWhoBoughtThisModel : GetWidgetModelBase<PeopleWhoBoughtThisAlsoBought>
    {
        private readonly IProductAnalyticsService _productAnalyticsService;
        private readonly CartModel _cart;

        public GetPeopleWhoBoughtThisModel(IProductAnalyticsService productAnalyticsService, CartModel cart)
        {
            _productAnalyticsService = productAnalyticsService;
            _cart = cart;
        }

        public override object GetModel(PeopleWhoBoughtThisAlsoBought widget)
        {
            var page = CurrentRequestData.CurrentPage;
            var model = new PeopleWhoBoughtThisAlsoBoughtViewModel { Title = String.Empty, Products = new List<Product>() };
            var product = page as Product;
            if (product != null)
            {
                model.Title = "People Who Bought '" + product.Name + "' Also Bought";
                model.Products = _productAnalyticsService.GetListOfProductsWhoWhereAlsoBought(product);
            }
            model.Cart = _cart;
            return model;
        }
    }
}