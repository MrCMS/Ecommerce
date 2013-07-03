using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductSearchService _productSearchService;

        public ProductController(IProductSearchService productSearchService)
        {
            _productSearchService = productSearchService;
        }

        public ViewResult Show(Product page, int pv=0)
        {
            ViewBag.Id = pv;
            ViewBag.ProductSearchUrl = UniquePageHelper.GetUrl<ProductSearch>();
            return View(page);
        }
    }
}