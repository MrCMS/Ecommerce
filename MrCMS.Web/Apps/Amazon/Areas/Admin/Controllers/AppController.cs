using System;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class AppController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;

        public AppController(IAmazonLogService amazonLogService, 
            IAmazonAnalyticsService amazonAnalyticsService)
        {
            _amazonLogService = amazonLogService;
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

        public PartialViewResult DashboardLogs(int page = 1)
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
        public JsonResult ProgressBarStatus(AmazonSyncModel model)
        {
            var progress = AmazonProgressBarHelper.GetStatus(model.Task);
            return Json(progress, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ProgressBarMessages(AmazonSyncModel model)
        {
            var progress = AmazonProgressBarHelper.Get(model.Task);
            return PartialView(new AmazonSyncModel()
                {
                    TaskId = model.Task,
                    Messages = progress.GetMessages(model.Page)
                });
        }
    }
}
