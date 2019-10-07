using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IBackInStockNotificationService _backInStockNotificationService;
        private readonly CartModel _cart;
        private readonly IProductUiService _productUiService;
        private readonly ITrackingService _trackingService;
        private readonly IWebpageAdminService _webpageAdminService;

        public ProductController(ITrackingService trackingService, IProductUiService productUiService,
            IBackInStockNotificationService backInStockNotificationService, CartModel cart,
            IWebpageAdminService webpageAdminService)
        {
            _trackingService = trackingService;
            _productUiService = productUiService;
            _backInStockNotificationService = backInStockNotificationService;
            _cart = cart;
            _webpageAdminService = webpageAdminService;
        }

        public ActionResult Show(Product page, int? variant)
        {
            _trackingService.AddItemToRecentlyViewedItemsCookie(page.Id);

            var variantToShow = _productUiService.GetVariantToShow(page, variant);

            if (!page.Variants.Any())
            {
                _webpageAdminService.Unpublish(page);
                return Redirect("/");
            }

            ViewData["selected-variant"] = variantToShow;

            ViewData["cart"] = _cart;

            ViewData["productreviews-enabled"] = MrCMSApplication.Get<ProductReviewSettings>().EnableProductReviews;

            return View(page);
        }


        [HttpPost]
        public RedirectResult BackInStock(BackInStockNotificationRequest request)
        {
            if (ModelState.IsValid)
            {
                _backInStockNotificationService.SaveRequest(request);
                TempData["back-in-stock"] = true;
                if (request.ProductVariant != null && request.ProductVariant.Product != null)
                    return
                        Redirect(string.Format("~/{0}?variant={1}", request.ProductVariant.Product.LiveUrlSegment,
                            request.ProductVariant.Id));
            }
            return Redirect(Referrer.ToString());
        }

        [HttpGet]
        public PartialViewResult BackInStockForm(ProductVariant productVariant)
        {
            ViewData["back-in-stock"] = _productUiService.UserNotifiedOfBackInStock(productVariant,
                TempData["back-in-stock"] is bool &&
                (bool) TempData["back-in-stock"]);
            var backInStockrequest = new BackInStockNotificationRequest {ProductVariant = productVariant};
            if (CurrentRequestData.CurrentUser != null)
                backInStockrequest.Email = CurrentRequestData.CurrentUser.Email;

            return PartialView(backInStockrequest);
        }
    }
}