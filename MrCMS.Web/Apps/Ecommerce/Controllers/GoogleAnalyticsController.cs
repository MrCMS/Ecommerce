using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class GoogleAnalyticsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly SEOSettings _seoSettings;
        private readonly IGoogleAnalyticsService _googleAnalyticsService;

        public GoogleAnalyticsController(SEOSettings seoSettings, IGoogleAnalyticsService googleAnalyticsService)
        {
            _seoSettings = seoSettings;
            _googleAnalyticsService = googleAnalyticsService;
        }

        public PartialViewResult GetEcommerceTrackingCode()
        {
            var page = CurrentRequestData.CurrentPage;
            var trackingScript = _seoSettings.TrackingScripts;
            
            if (page is OrderPlaced)
            {
                var order = TempData["Order"] as Order;
                if (order != null && order.CreatedOn > CurrentRequestData.Now.AddSeconds(-15))
                {
                    trackingScript = _googleAnalyticsService.GetAnalayticsCode(order, trackingScript);
                }
            }
            return PartialView("AnalyticsCode", trackingScript);
        }
    }
}
