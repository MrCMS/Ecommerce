using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
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
        private readonly IBackInStockNotificationService _backInStockNotificationService;

        public ProductController(ITrackingService trackingService, IProductVariantService productVariantService, IBackInStockNotificationService backInStockNotificationService)
        {
            _trackingService = trackingService;
            _productVariantService = productVariantService;
            _backInStockNotificationService = backInStockNotificationService;
        }

        public ViewResult Show(Product page, int? variant)
        {
            _trackingService.AddItemToRecentlyViewedItemsCookie(page.Id);
            ViewData["back-in-stock"] = TempData["back-in-stock"];
            if (variant.HasValue)
                ViewData["selected-variant"] = _productVariantService.Get(variant.Value);
            return View(page);
        }

        [HttpPost]
        public RedirectResult BackInStock(BackInStockNotificationRequest request)
        {
            _backInStockNotificationService.SaveRequest(request);
            TempData["back-in-stock"] = true;
            if (request.ProductVariant != null && request.ProductVariant.Product != null)
                return Redirect(string.Format("~/{0}", request.ProductVariant.Product.LiveUrlSegment));
            return Redirect(Referrer.ToString());
        }
    }
}