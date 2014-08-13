using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Collections.Generic;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductSearchController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductSearchIndexService _productSearchIndexService;
        private readonly CartModel _cart;
        private readonly IProductSearchQueryService _productSearchQueryService;

        public ProductSearchController(IProductSearchIndexService productSearchIndexService, CartModel cart,
            IProductSearchQueryService productSearchQueryService)
        {
            _productSearchIndexService = productSearchIndexService;
            _cart = cart;
            _productSearchQueryService = productSearchQueryService;
        }

        public ViewResult Show(ProductSearch page, [IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            ViewData["query"] = query;
            SetViewData(query);
            return View(page);
        }

        private void SetViewData(ProductSearchQuery query)
        {
            _productSearchQueryService.SetViewData(query, ViewData);
            ViewData["product-price-range-min"] = 0;
            ViewData["product-price-range-max"] = 5000;
        }

        public PartialViewResult Query([IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            SetViewData(query);
            return PartialView(query);
        }

        [HttpGet]
        public PartialViewResult Results([IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            ViewData["query"] = query;
            ViewData["cart"] = _cart;
            return PartialView(_productSearchIndexService.SearchProducts(query));
        }
    }

    public interface IProductSearchQueryService
    {
        void SetViewData(ProductSearchQuery query, ViewDataDictionary viewData);
    }

    public class ProductSearchQueryService : IProductSearchQueryService
    {
        private readonly IProductOptionManager _productOptionManager;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;

        public ProductSearchQueryService(IProductOptionManager productOptionManager, IBrandService brandService, ICategoryService categoryService)
        {
            _productOptionManager = productOptionManager;
            _brandService = brandService;
            _categoryService = categoryService;
        }

        public void SetViewData(ProductSearchQuery query, ViewDataDictionary viewData)
        {
            var productOptionSearchData = _productOptionManager.GetSearchData(query);
            viewData["product-options"] = productOptionSearchData.AttributeOptions;
            viewData["product-specifications"] = productOptionSearchData.SpecificationOptions;
            viewData["product-brands"] = _brandService.GetAvailableBrands(query);
            viewData["categories"] = _categoryService.GetCategoriesForSearch(query);

        }
    }
}
