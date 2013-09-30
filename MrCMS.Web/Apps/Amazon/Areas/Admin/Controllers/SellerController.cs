using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Models;
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
        public ActionResult Settings_POST(AmazonSellerSettings amazonSellerSettings)
        {
            _amazonLogService.Add(AmazonLogType.SellerSettings, AmazonLogStatus.Update,null,null, null,null,null,null,null,null);
            _configurationProvider.SaveSettings(amazonSellerSettings);
            return View(amazonSellerSettings);
        }
    }
}
