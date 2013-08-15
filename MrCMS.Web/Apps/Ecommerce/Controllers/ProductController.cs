using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ITrackingService _trackingService;
        private readonly IProductVariantService _productVariantService;

        public ProductController(ITrackingService trackingService, IProductVariantService productVariantService)
        {
            _trackingService = trackingService;
            _productVariantService = productVariantService;
        }

        public ViewResult Show(Product page, int? variant)
        {
            _trackingService.AddItemToRecentlyViewedItemsCookie(page.Id);
            if (variant.HasValue)
                ViewData["selected-variant"] = _productVariantService.Get(variant.Value);
            return View(page);
        }
    }
}