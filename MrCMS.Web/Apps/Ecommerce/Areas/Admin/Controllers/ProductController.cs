using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductService _productService;
        private readonly IDocumentService _documentService;
        private readonly ICategoryService _categoryService;
        private readonly ITaxRateManager _taxRateManager;

        public ProductController(IProductService productService, IDocumentService documentService, ICategoryService categoryService, ITaxRateManager taxRateManager)
        {
            _productService = productService;
            _documentService = documentService;
            _categoryService = categoryService;
            _taxRateManager = taxRateManager;
        }

        /// <summary>
        /// Lists products
        /// </summary>
        /// <param name="q">query string to filter by</param>
        /// <param name="p">page number</param>
        /// <returns></returns>
        public ViewResult Index(string q = null, int p = 1)
        {
            if (_documentService.GetUniquePage<ProductSearch>() == null)
                return View();
            var searchResult = _productService.Search(q, p);
            return View(searchResult);
        }

        [HttpGet]
        public PartialViewResult MakeMultiVariant(Product product)
        {
            return PartialView(new MakeMultivariantModel { ProductId = product.Id });
        }

        [HttpPost]
        public RedirectToRouteResult MakeMultiVariant(MakeMultivariantModel model)
        {
            _productService.MakeMultiVariant(model);

            return RedirectToAction("Edit", "Webpage", new { id = model.ProductId });
        }

        [HttpGet]
        public PartialViewResult Categories(Product product)
        {
            return PartialView(product);
        }

        [HttpGet]
        public PartialViewResult AddCategory(Product product, string query, int page = 1)
        {
            ViewData["product"] = product;
            var categories = _categoryService.GetCategories(product, query, page);
            return PartialView(categories);
        }

        [HttpPost]
        public JsonResult AddCategory(Product product, int categoryId)
        {
            try
            {
                _productService.AddCategory(product, categoryId);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpGet]
        public PartialViewResult RemoveCategory(Product product, int categoryId)
        {
            ViewData["category"] = _documentService.GetDocument<Category>(categoryId);
            return PartialView(product);
        }

        [HttpPost]
        [ActionName("RemoveCategory")]
        public RedirectToRouteResult RemoveCategory_POST(Product product, int categoryId)
        {
            _productService.RemoveCategory(product, categoryId);

            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }

        public PartialViewResult PricingButtons(Product product)
        {
            return PartialView(product);
        }

        public PartialViewResult PricingInfo(Product product)
        {
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(product.TaxRate);
            return PartialView(product);
        }

        public PartialViewResult Specifications(Product product)
        {
            return PartialView(product);
        }

        public PartialViewResult Images(Product product)
        {
            return PartialView(product);
        }

        public PartialViewResult UploadImage(Product product)
        {
            return PartialView(product);
        }
    }
}