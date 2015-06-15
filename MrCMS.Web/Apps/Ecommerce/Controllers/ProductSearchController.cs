using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Services.Caching;
using MrCMS.Web.Apps.Ecommerce.Filters;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductSearchController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IHtmlCacheService _htmlCacheService;
        private readonly IProductSearchIndexService _productSearchIndexService;

        public ProductSearchController(IProductSearchIndexService productSearchIndexService, CartModel cart,
            IHtmlCacheService htmlCacheService)
        {
            _productSearchIndexService = productSearchIndexService;
            _cart = cart;
            _htmlCacheService = htmlCacheService;
        }

        public ViewResult Show(ProductSearch page,
            [IoCModelBinder(typeof (ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            ViewData["query"] = query;
            return View(page);
        }

        public ActionResult Query([IoCModelBinder(typeof (ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            return _htmlCacheService.GetContent(this, _productSearchIndexService.GetCachingInfo(query, "-query"),
                helper => helper.Action("QueryInternal", new {query}));
        }

        [SetProductSearchViewData]
        public PartialViewResult QueryInternal(ProductSearchQuery query)
        {
            return PartialView("Query", query);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        public ActionResult Results([IoCModelBinder(typeof (ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            return _htmlCacheService.GetContent(this, _productSearchIndexService.GetCachingInfo(query, "-results"),
                helper => helper.Action("ResultsInternal", new {query}));
        }

        [SetProductResultsViewData]
        public PartialViewResult ResultsInternal(ProductSearchQuery query)
        {
            ViewData["query"] = query;
            ViewData["cart"] = _cart;
            return PartialView("Results", _productSearchIndexService.SearchProducts(query));
        }
    }
}