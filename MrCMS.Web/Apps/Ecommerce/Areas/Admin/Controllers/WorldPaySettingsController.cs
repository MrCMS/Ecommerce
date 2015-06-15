using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Payment.WorldPay;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class WorldPaySettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public WorldPaySettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [MrCMSACLRule(typeof(WorldPaySettingsACL), WorldPaySettingsACL.View)]
        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<WorldPaySettings>());
        }

        [HttpPost]
        [MrCMSACLRule(typeof(WorldPaySettingsACL), WorldPaySettingsACL.View)]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(WorldPaySettingsModelBinder))] WorldPaySettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("WorldPay Settings saved successfully.");
            return RedirectToAction("Index");
        }
    }
}