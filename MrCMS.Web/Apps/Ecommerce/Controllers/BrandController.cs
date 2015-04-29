using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Filters;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class BrandController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IProductSearchIndexService _productSearchIndexService;

        public BrandController(CartModel cart, IProductSearchIndexService productSearchIndexService)
        {
            _cart = cart;
            _productSearchIndexService = productSearchIndexService;
        }

        public ViewResult Show(Brand page,
            [IoCModelBinder(typeof(ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            query.BrandId = page.Id;
            ViewData["query"] = query;
            return View(page);
        }

        [SetBrandSearchViewData]
        public ActionResult Query([IoCModelBinder(typeof(ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            return PartialView("Query", query);
        }

        [SetProductResultsViewData]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        public ActionResult Results([IoCModelBinder(typeof(ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            ViewData["query"] = query;
            ViewData["cart"] = _cart;
            return PartialView("Results", _productSearchIndexService.SearchProducts(query));
        }

    }
}