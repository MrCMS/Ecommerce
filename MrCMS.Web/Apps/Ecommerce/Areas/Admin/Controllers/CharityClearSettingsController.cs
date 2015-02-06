using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Payment.CharityClear;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CharityClearSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public CharityClearSettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<CharityClearSettings>());
        }

        [HttpPost]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(CharityClearSettingsModelBinder))] CharityClearSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("Settings saved successfully.");
            return RedirectToAction("Index");
        }
    }
}