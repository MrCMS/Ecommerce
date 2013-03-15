using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductService _productService;
        private readonly IDocumentService _documentService;

        public ProductController(IProductService productService, IDocumentService documentService)
        {
            _productService = productService;
            _documentService = documentService;
        }

        /// <summary>
        /// Lists products
        /// </summary>
        /// <param name="q">query string to filter by</param>
        /// <param name="p">page number</param>
        /// <returns></returns>
        public ViewResult Index(string q = null, int p = 1)
        {
            if (_documentService.GetUniquePage<ProductContainer>() == null)
                return View();
            var searchResult = _productService.Search(q, p);
            return View(searchResult);
        }

        [HttpGet]
        public PartialViewResult MakeMultiVariant(Product product)
        {
            return PartialView(product);
        }

        [HttpPost]
        public RedirectToRouteResult MakeMultiVariant(Product product, string option1, string option2, string option3)
        {
            _productService.MakeMultiVariant(product, option1, option2, option3);

            return RedirectToAction("Edit", "Webpage", new {id = product.Id});
        }
    }
}