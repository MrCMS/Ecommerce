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

        public ViewResult Show(Product page, int pv=0)
        {
            if(_productVariantService.Get(pv)!=null)
                ViewBag.Id = pv;
            else
                ViewBag.Id = 0;
            ViewBag.ProductSearchUrl = UniquePageHelper.GetUrl<ProductSearch>();
            return View(page);
        }
    }
}