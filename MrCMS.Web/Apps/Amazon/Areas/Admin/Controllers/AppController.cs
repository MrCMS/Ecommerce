using MrCMS.Settings;
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
            _configurationProvider.SaveSettings(amazonAppSettings);
            return RedirectToAction("Settings");
        }
    }
}
