using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductSearchQueryService : IProductSearchQueryService
    {
        private readonly IAvailableBrandsService _availableBrandsService;
        private readonly ICategoryService _categoryService;
        private readonly IProductSearchIndexService _productSearchIndexService;
        private readonly IProductOptionManager _productOptionManager;

        public ProductSearchQueryService(IProductOptionManager productOptionManager, IAvailableBrandsService availableBrandsService,
            ICategoryService categoryService,IProductSearchIndexService productSearchIndexService)
        {
            _productOptionManager = productOptionManager;
            _availableBrandsService = availableBrandsService;
            _categoryService = categoryService;
            _productSearchIndexService = productSearchIndexService;
        }

        public void SetViewData(ProductSearchQuery query, ViewDataDictionary viewData)
        {
            ProductOptionSearchData productOptionSearchData = _productOptionManager.GetSearchData(query);
            viewData["product-options"] = productOptionSearchData.AttributeOptions;
            viewData["product-specifications"] = productOptionSearchData.SpecificationOptions;
            viewData["product-brands"] = _availableBrandsService.GetAvailableBrands(query);
            viewData["categories"] = _categoryService.GetCategoriesForSearch(query);
            viewData["max-price"] = _productSearchIndexService.GetMaxPrice(query);
        }
    }
}