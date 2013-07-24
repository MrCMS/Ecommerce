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
        private readonly IProductVariantService _productVariantService;

        public ProductController(IProductSearchService productSearchService, IProductVariantService productVariantService)
        {
            _productSearchService = productSearchService;
            _productVariantService = productVariantService;
        }

        public ViewResult Show(Product page, int? variant)
        {
            if (variant.HasValue)
                ViewData["selected-variant"] = _productVariantService.Get(variant.Value);
            return View(page);
        }
    }
}