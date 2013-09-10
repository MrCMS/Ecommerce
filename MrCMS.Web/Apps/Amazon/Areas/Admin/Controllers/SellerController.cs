using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class SellerController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAmazonLogService _amazonLogService;
        private readonly AmazonSellerSettings _amazonSellerSettings;

        public SellerController(IConfigurationProvider configurationProvider,
            IAmazonLogService amazonLogService,
            AmazonSellerSettings amazonSellerSettings)
        {
            _configurationProvider = configurationProvider;
            _amazonLogService = amazonLogService;
            _amazonSellerSettings = amazonSellerSettings;
        }

        [HttpGet]
        public ActionResult Settings()
        {
            return View(_amazonSellerSettings);
        }

        [HttpPost]
        [ActionName("Settings")]
        public RedirectToRouteResult Settings_POST(AmazonSellerSettings amazonSellerSettings)
        {
            _configurationProvider.SaveSettings(amazonSellerSettings);
            return RedirectToAction("Settings");
        }
    }
}
