using System;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Search
{
    public class GetProductSearchView : IGetProductSearchView
    {
        private const string ProductSearchView = "mrcms-product-search-view";

        public ProductSearchView Get()
        {
            var cookie = CookieHelper.GetValue(ProductSearchView);
            if (string.IsNullOrWhiteSpace(cookie))
            {
                Set(Models.ProductSearchView.Grid);
            }
            if (cookie != null)
                return (ProductSearchView)Enum.Parse(typeof(ProductSearchView), cookie);

            return Models.ProductSearchView.Grid;
        }

        public void Set(ProductSearchView productSearchView)
        {
            CookieHelper.UpdateValue(ProductSearchView, productSearchView.ToString());
        }
    }
}