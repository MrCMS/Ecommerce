using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Helpers;
using MrCMS.Services.Caching;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Caching;
using MrCMS.Website.Controllers;
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
        private readonly IHtmlCacheService _htmlCacheService;

        public ProductSearchController(IProductSearchIndexService productSearchIndexService, CartModel cart,
            IProductSearchQueryService productSearchQueryService, IHtmlCacheService htmlCacheService)
        {
            _productSearchIndexService = productSearchIndexService;
            _cart = cart;
            _productSearchQueryService = productSearchQueryService;
            _htmlCacheService = htmlCacheService;
        }

        public ViewResult Show(ProductSearch page, [IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            ViewData["query"] = query;
            //SetViewData(query);
            return View(page);
        }

        private void SetViewData(ProductSearchQuery query)
        {
            _productSearchQueryService.SetViewData(query, ViewData);
            ViewData["product-price-range-min"] = 0;
            ViewData["product-price-range-max"] = 5000;
        }

        public ActionResult Query([IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            return _htmlCacheService.GetContent(this, _productSearchQueryService.GetCachingInfo(query),
                helper => helper.Action("QueryInternal", new {query}));
        }

        public PartialViewResult QueryInternal(ProductSearchQuery query)
        {
            SetViewData(query);
            return PartialView("Query", query);
        }

        [HttpGet]
        public ActionResult Results([IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            return _htmlCacheService.GetContent(this, _productSearchQueryService.GetCachingInfo(query),
                helper => helper.Action("ResultsInternal", new {query}));
        }

        public PartialViewResult ResultsInternal(ProductSearchQuery query)
        {
            ViewData["query"] = query;
            ViewData["cart"] = _cart;
            return PartialView("Results", _productSearchIndexService.SearchProducts(query));
        }
    }
}
