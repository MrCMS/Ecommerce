using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class SettingsController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAmazonLogService _amazonLogService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly AmazonSyncSettings _amazonSyncSettings;

        public SettingsController(IConfigurationProvider configurationProvider,
            IAmazonLogService amazonLogService, 
            AmazonAppSettings amazonAppSettings, 
            AmazonSellerSettings amazonSellerSettings, 
            AmazonSyncSettings amazonSyncSettings)
        {
            _configurationProvider = configurationProvider;
            _amazonLogService = amazonLogService;
            _amazonAppSettings = amazonAppSettings;
            _amazonSellerSettings = amazonSellerSettings;
            _amazonSyncSettings = amazonSyncSettings;
        }

        [HttpGet]
        public ActionResult App()
        {
            return View(_amazonAppSettings);
        }

        [HttpPost]
        [ActionName("App")]
        public ActionResult App_POST(AmazonAppSettings amazonAppSettings)
        {
            _amazonLogService.Add(AmazonLogType.AppSettings, AmazonLogStatus.Update,null,null,null,null,null,null,null);
            _configurationProvider.SaveSettings(amazonAppSettings);
            return View(amazonAppSettings);
        }

        [HttpGet]
        public ActionResult Seller()
        {
            return View(_amazonSellerSettings);
        }

        [HttpPost]
        [ActionName("Seller")]
        public ActionResult Seller_POST(AmazonSellerSettings amazonSellerSettings)
        {
            _amazonLogService.Add(AmazonLogType.SellerSettings, AmazonLogStatus.Update, null, null, null, null, null, null, null, null);
            _configurationProvider.SaveSettings(amazonSellerSettings);
            return View(amazonSellerSettings);
        }

        [HttpGet]
        public ActionResult Sync()
        {
            return View(_amazonSyncSettings);
        }

        [HttpPost]
        [ActionName("Sync")]
        public ActionResult Sync_POST(AmazonSyncSettings amazonSyncSettings)
        {
            if (_amazonSyncSettings != null)
            {
                _amazonSyncSettings.TryCalculateVat = amazonSyncSettings.TryCalculateVat;
                _amazonSyncSettings.UseDefaultTaxRateForShippingTax = amazonSyncSettings.UseDefaultTaxRateForShippingTax;
                _amazonLogService.Add(AmazonLogType.SyncSettings, AmazonLogStatus.Update, null, null, null, null, null,
                                      null, null, null);
                _configurationProvider.SaveSettings(_amazonSyncSettings);

                return View(_amazonSyncSettings);
            }
            return RedirectToAction("Dashboard", "App");
        }
    }
}
