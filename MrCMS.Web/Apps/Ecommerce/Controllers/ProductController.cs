using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductSearchService _productSearchService;

        public ProductController(IProductSearchService productSearchService)
        {
            _productSearchService = productSearchService;
        }

        public ViewResult Show(Product page)
        {
            ViewBag.ProductSearchUrl = _productSearchService.GetSiteProductSearch().LiveUrlSegment;
            return View(page);
        }
    }
}