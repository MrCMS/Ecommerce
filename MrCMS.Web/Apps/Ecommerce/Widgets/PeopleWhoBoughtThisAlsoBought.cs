using System;
using System.Collections.Generic;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics.Products;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class PeopleWhoBoughtThisAlsoBought : Widget 
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var page = CurrentRequestData.CurrentPage;
            var model = new PeopleWhoBoughtThisAlsoBoughtViewModel { Title = String.Empty, Products = new List<Product>() };
            if (page is Product)
            {
                var product = (Product) page;
                model.Title = "People Who Bought '" + product.Name + "' Also Bought";
                model.Products = MrCMSApplication.Get<IProductAnalyticsService>().GetListOfProductsWhoWhereAlsoBought(product);
            }
            return model;
        }
    }
    public class PeopleWhoBoughtThisAlsoBoughtViewModel
    {
        public IList<Product> Products { get; set; }
        public string Title { get; set; }
    }
}