using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Services.Caching;
using MrCMS.Web.Apps.Ecommerce.Filters;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductSearchController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IHtmlCacheService _htmlCacheService;
        private readonly IProductSearchIndexService _productSearchIndexService;
        private IProductPricingMethod pricingMethod;

        public ProductSearchController(IProductSearchIndexService productSearchIndexService, CartModel cart,
                                       IHtmlCacheService htmlCacheService)
        {
            _productSearchIndexService = productSearchIndexService;
            _cart = cart;
            _htmlCacheService = htmlCacheService;

             pricingMethod = MrCMSApplication.Get<IProductPricingMethod>();
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

        public ActionResult GetProductSuggestion(string searchTerm)
        {
            return Json(GetMatchingProduct(searchTerm), JsonRequestBehavior.AllowGet);
        }

        private IList<ProductSearchSuggestionItem> GetMatchingProduct(string query)
        {
            IList<ProductSearchSuggestionItem> suggestionItems = new List<ProductSearchSuggestionItem>();
            
            
var productsQuery = _productSearchIndexService.SearchProducts(new ProductSearchQuery());          
                        
            if (productsQuery != null && productsQuery.Count > 0)
            {
               var productList = productsQuery.ToList()
                                              .Where(prod => prod.Name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                                              .ToList();

               // Filter the product that conain the serach term
               productList.ForEach(p => suggestionItems.Add(new ProductSearchSuggestionItem()
               {
                   ProductName = p.Name,
                   ProductPrice = pricingMethod.GetDisplayPrice(p).ToString(),
                   ImageDisplayUrl = p.DisplayImageUrl
               }));
            }

            return suggestionItems;
        }
    }
}