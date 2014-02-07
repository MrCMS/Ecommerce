using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Settings;
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

    public class ProductSearchQueryModelBinder : MrCMSDefaultModelBinder
    {
        public ProductSearchQueryModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.IsChildAction)
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            var model = new ProductSearchQuery
                            {
                                Specifications =
                                    (controllerContext.HttpContext.Request["Specifications"] ??
                                     string.Empty).Split(new[] { '|' },
                                                         StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(s => Convert.ToInt32(s))
                                                  .ToList(),
                                Options =
                                    (controllerContext.HttpContext.Request["Options"] ?? string.Empty)
                                    .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => Convert.ToInt32(s))
                                    .ToList(),
                                PageSize = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["PageSize"])
                                               ? Convert.ToInt32(controllerContext.HttpContext.Request["PageSize"])
                                               : MrCMSApplication.Get<EcommerceSettings>()
                                                                 .ProductPerPageOptions.FirstOrDefault(),
                                Page = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["Page"])
                                           ? Convert.ToInt32(controllerContext.HttpContext.Request["Page"])
                                           : 1,
                                CategoryId =
                                    !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["CategoryId"])
                                        ? Convert.ToInt32(controllerContext.HttpContext.Request["CategoryId"])
                                        : (int?)null,
                                PriceFrom =
                                    !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["PriceFrom"])
                                        ? Convert.ToDouble(controllerContext.HttpContext.Request["PriceFrom"])
                                        : 0,
                                PriceTo = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["PriceTo"])
                                              ? Convert.ToDouble(controllerContext.HttpContext.Request["PriceTo"])
                                              : (double?)null,
                                BrandId =
                                    !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["BrandId"])
                                        ? Convert.ToInt32(controllerContext.HttpContext.Request["BrandId"])
                                        : (int?)null,
                                SearchTerm = controllerContext.HttpContext.Request["SearchTerm"]
                            };

            model.SortBy = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["SortBy"])
                               ? (ProductSearchSort)Convert.ToInt32(controllerContext.HttpContext.Request["SortBy"])
                               : model.SortBy;

            return model;
        }
    }
}
