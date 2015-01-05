using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductSearchResultsService : IProductSearchResultsService
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IGetProductSearchSortByValue _getProductSearchSortByValue;
        private readonly ISession _session;

        public ProductSearchResultsService(
            IGetProductSearchSortByValue getProductSearchSortByValue,
            EcommerceSettings ecommerceSettings, ISession session)
        {
            _getProductSearchSortByValue = getProductSearchSortByValue;
            _ecommerceSettings = ecommerceSettings;
            _session = session;
        }

        public void SetViewData(ProductSearchQuery productSearchQuery, ViewDataDictionary viewData)
        {
            viewData["sort-by-options"] = GetSortByOptions(productSearchQuery);
            viewData["per-page-options"] = GetPerPageOptions(productSearchQuery);
        }

        private object GetPerPageOptions(ProductSearchQuery query)
        {
            IEnumerable<int> options = _ecommerceSettings
                .SearchProductsPerPage.Split(',')
                .Where(s =>
                {
                    int result;
                    return int.TryParse(s, out result);
                }).Select(s => Convert.ToInt32(s));
            return options.BuildSelectItemList(i => string.Format("{0} products per page", i), i => i.ToString(),
                i => i == query.PageSize, emptyItem: null);
        }

        private List<SelectListItem> GetSortByOptions(ProductSearchQuery query)
        {
            var productSearchSorts = new List<ProductSearchSort>
            {
                ProductSearchSort.MostPopular,
                ProductSearchSort.Latest,
                ProductSearchSort.NameAToZ,
                ProductSearchSort.NameZToA,
                ProductSearchSort.PriceLowToHigh,
                ProductSearchSort.PriceHighToLow,
            };
            if (query.CategoryId.HasValue && _getProductSearchSortByValue.CategoryHasBeenSorted(query))
                productSearchSorts.Insert(0, ProductSearchSort.DisplayOrder);
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                productSearchSorts.Insert(0, ProductSearchSort.Relevance);
            ProductSearchSort productSearchSort = _getProductSearchSortByValue.Get(query);
            return productSearchSorts.BuildSelectItemList(sort => sort.GetDescription(),
                sort => Convert.ToInt32(sort).ToString(),
                sort => sort == productSearchSort, emptyItem: null);
        }

 
    }
}