using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
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

        public AppController(IConfigurationProvider configurationProvider,
            IAmazonLogService amazonLogService, 
            AmazonAppSettings amazonAppSettings)
        {
            _configurationProvider = configurationProvider;
            _amazonLogService = amazonLogService;
            _amazonAppSettings = amazonAppSettings;
        }

        [HttpGet]
        public ViewResult Dashboard()
        {
            return View();
        }

        public ActionResult DashboardLogs(int page = 1)
        {
            var model = new AmazonDashboardModel()
            {
                Logs = _amazonLogService.GetEntriesPaged(page)
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
