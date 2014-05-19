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
        private readonly ICategoryService _categoryService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IProductSearchService _productSearchService;
        private readonly IBrandService _brandService;
        private readonly CartModel _cart;

        public ProductSearchController(ICategoryService categoryService,
                                       IProductOptionManager productOptionManager,
                                       IProductSearchService productSearchService,
                                       IBrandService brandService, CartModel cart)
        {
            _categoryService = categoryService;
            _productOptionManager = productOptionManager;
            _productSearchService = productSearchService;
            _brandService = brandService;
            _cart = cart;
        }

        public ViewResult Show(ProductSearch page, [IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            ViewData["query"] = query;
            SetViewData(query);
            return View(page);
        }

        private void SetViewData(ProductSearchQuery query)
        {
            ViewData["product-options"] = _productOptionManager.GetSearchAttributeOptions(query);
            ViewData["product-specifications"] = _productOptionManager.GetSearchSpecificationAttributes(query);
            ViewData["product-brands"] = _brandService.GetAvailableBrands(query);
            ViewData["product-price-range-min"] = 0;
            ViewData["product-price-range-max"] = 5000;
            ViewData["categories"] = _categoryService.GetCategoriesForSearch(query);
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
            return PartialView(_productSearchService.SearchProducts(query));
        }
    }
}
