using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web.Mvc;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly ICurrencyService _currencyService;

        public EcommerceSettingsController(IConfigurationProvider configurationProvider, EcommerceSettings ecommerceSettings,
             ICurrencyService currencyService)
        {
            _configurationProvider = configurationProvider;
            _ecommerceSettings = ecommerceSettings;
            _currencyService = currencyService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(EcommerceSettingsACL), EcommerceSettingsACL.Edit)]
        public ActionResult Edit()
        {
            ViewData["currency-options"] = _currencyService.Options(_ecommerceSettings.CurrencyId);
            return View(_ecommerceSettings);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(EcommerceSettingsACL), EcommerceSettingsACL.Edit)]
        public RedirectToRouteResult Edit_POST(EcommerceSettings ecommerceSettings)
        {
            _configurationProvider.SaveSettings(ecommerceSettings);
            TempData.SuccessMessages().Add("Ecommerce Settings Saved");
            return RedirectToAction("Edit");
        }
    }
}
