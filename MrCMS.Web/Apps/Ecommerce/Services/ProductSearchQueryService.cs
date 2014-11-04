using System;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductSearchQueryService : IProductSearchQueryService
    {
        private readonly IProductOptionManager _productOptionManager;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly EcommerceSearchCacheSettings _ecommerceSettings;

        public ProductSearchQueryService(IProductOptionManager productOptionManager, IBrandService brandService, ICategoryService categoryService, EcommerceSearchCacheSettings ecommerceSettings)
        {
            _productOptionManager = productOptionManager;
            _brandService = brandService;
            _categoryService = categoryService;
            _ecommerceSettings = ecommerceSettings;
        }

        public void SetViewData(ProductSearchQuery query, ViewDataDictionary viewData)
        {
            var productOptionSearchData = _productOptionManager.GetSearchData(query);
            viewData["product-options"] = productOptionSearchData.AttributeOptions;
            viewData["product-specifications"] = productOptionSearchData.SpecificationOptions;
            viewData["product-brands"] = _brandService.GetAvailableBrands(query);
            viewData["categories"] = _categoryService.GetCategoriesForSearch(query);

        }

        public CachingInfo GetCachingInfo(ProductSearchQuery query, string suffix = null)
        {
            return new CachingInfo(_ecommerceSettings.SearchCache, GetCacheKey(query) + suffix,
                TimeSpan.FromSeconds(_ecommerceSettings.SearchCacheLength), _ecommerceSettings.SearchCacheExpiryType);
        }

        private string GetCacheKey(ProductSearchQuery query)
        {
            return _ecommerceSettings.SearchCachePerUser
                ? JsonConvert.SerializeObject(new { CurrentRequestData.UserGuid, query })
                : JsonConvert.SerializeObject(query);
        }
    }
}