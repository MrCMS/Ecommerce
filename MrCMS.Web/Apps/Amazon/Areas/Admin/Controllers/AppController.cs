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
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;

        public AppController(IConfigurationProvider configurationProvider,
            IAmazonLogService amazonLogService, 
            AmazonAppSettings amazonAppSettings, 
            IAmazonAnalyticsService amazonAnalyticsService)
        {
            _configurationProvider = configurationProvider;
            _amazonLogService = amazonLogService;
            _amazonAppSettings = amazonAppSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
        }

        [HttpGet]
        public ViewResult Dashboard(DateTime? filterFrom, DateTime? filterUntil)
        {
            return View(_amazonAnalyticsService.GetAmazonDashboardModel(filterFrom, filterUntil));
        }

        [HttpPost]
        [ActionName("Dashboard")]
        public ViewResult Dashboard_POST(DateTime? filterFrom, DateTime? filterUntil)
        {
            return View(_amazonAnalyticsService.GetAmazonDashboardModel(filterFrom, filterUntil));
        }

        public ActionResult DashboardLogs(int page = 1)
        {
            var model = new AmazonDashboardModel()
            {
                Logs = _amazonLogService.GetEntriesPaged(page,null,null,5)
            };
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult Revenue(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue)
            {
                return Json(_amazonAnalyticsService.GetRevenue(from.Value,to.Value));
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult ProductsSold(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue)
            {
                return Json(_amazonAnalyticsService.GetProductsSold(from.Value, to.Value));
            }
            return Json(false);
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
            return RedirectToAction("Dashboard");
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
