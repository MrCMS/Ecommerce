using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lists products
        /// </summary>
        /// <param name="q">query string to filter by</param>
        /// <param name="p">page number</param>
        /// <returns></returns>
        public ViewResult Index(string q = null, int p = 1)
        {
            var searchResult = _productService.Search(q, p);
            return View(searchResult);
        }
    }
}