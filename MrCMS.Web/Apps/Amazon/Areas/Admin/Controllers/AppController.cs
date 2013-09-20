using System;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class AppController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAmazonLogService _amazonLogService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;

        public AppController(IConfigurationProvider configurationProvider,
            IAmazonLogService amazonLogService, 
            AmazonAppSettings amazonAppSettings, 
            IAmazonAnalyticsService amazonAnalyticsService, 
            AmazonSellerSettings amazonSellerSettings)
        {
            _configurationProvider = configurationProvider;
            _amazonLogService = amazonLogService;
            _amazonAppSettings = amazonAppSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonSellerSettings = amazonSellerSettings;
        }

        [HttpGet]
        public ViewResult Dashboard(DateTime? filterFrom, DateTime? filterUntil)
        {
            return View(InitDashboard(filterFrom, filterUntil));
        }

        [HttpPost]
        [ActionName("Dashboard")]
        public ViewResult Dashboard_POST(DateTime? filterFrom, DateTime? filterUntil)
        {
            return View(InitDashboard(filterFrom, filterUntil));
        }

        private AmazonDashboardModel InitDashboard(DateTime? from,DateTime? to)
        {
            var model = new AmazonDashboardModel();
            if (from.HasValue)
                model.FilterFrom = from.Value;
            if (to.HasValue)
                model.FilterUntil = to.Value;
            model.NoOfActiveListings = _amazonAnalyticsService.GetNumberOfActiveListings();
            model.NoOfApiCalls = _amazonAnalyticsService.GetNumberOfApiCalls(model.FilterFrom, model.FilterUntil);
            model.NoOfOrders = _amazonAnalyticsService.GetNumberOfOrders(model.FilterFrom, model.FilterUntil);
            model.NoOfUnshippedOrders = _amazonAnalyticsService.GetNumberUnshippedOrders(model.FilterFrom, model.FilterUntil);
            model.AverageOrderAmount = _amazonAnalyticsService.GetAverageOrderAmount(model.FilterFrom,model.FilterUntil);
            model.NoOfOrderedProducts = _amazonAnalyticsService.GetNumberOfOrderedProducts(model.FilterFrom, model.FilterUntil);
            model.NoOfShippedProducts = _amazonAnalyticsService.GetNumberOfShippedProducts(model.FilterFrom, model.FilterUntil);
            model.AppSettingsStatus = AmazonAppHelper.GetAmazonAppSettingsStatus(_amazonAppSettings);
            model.SellerSettingsStatus = AmazonAppHelper.GetAmazonSellerSettingsStatus(_amazonSellerSettings);
            return model;
        }

        public ActionResult DashboardLogs(int page = 1)
        {
            var model = new AmazonDashboardModel()
            {
                Logs = _amazonLogService.GetEntriesPaged(page,null,null,5)
            };
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Settings()
        {
            return View(_amazonAppSettings);
        }

        [HttpPost]
        [ActionName("Settings")]
        public RedirectToRouteResult Settings_POST(AmazonAppSettings amazonAppSettings)
        {
            _amazonLogService.Add(AmazonLogType.AppSettings, AmazonLogStatus.Update);
            _configurationProvider.SaveSettings(amazonAppSettings);
            return RedirectToAction("Settings");
        }

        [HttpGet]
        public JsonResult ProgressBarStatus(AmazonSyncModel model)
        {
            var progress = AmazonProgressBarHelper.GetStatus(model.TaskId);
            return Json(progress, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ProgressBarMessages(AmazonSyncModel model)
        {
            if (model!=null && model.TaskId.HasValue)
            {
                var progress = AmazonProgressBarHelper.GetProgressBar(model.TaskId);
                return PartialView(new AmazonSyncModel()
                {
                    TaskId = model.TaskId,
                    Messages = progress.GetMessages(model.Page)
                });
            }
            return PartialView(null);
        }
    }
}
