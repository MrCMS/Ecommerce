using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web.Mvc;
using MrCMS.Website.Filters;
using NHibernate.Mapping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IEcommerceSettingsAdminService _ecommerceSettingsAdminService;

        public EcommerceSettingsController( IEcommerceSettingsAdminService ecommerceSettingsAdminService)
       {
            _ecommerceSettingsAdminService = ecommerceSettingsAdminService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(EcommerceSettingsACL), EcommerceSettingsACL.Edit)]
        public ActionResult Edit()
        {
            ViewData["currency-options"] = _ecommerceSettingsAdminService.GetCurrencyOptions();
            ViewData["default-sort-options"] = _ecommerceSettingsAdminService.GetDefaultSortOptions();
            return View(_ecommerceSettingsAdminService.GetSettings());
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(EcommerceSettingsACL), EcommerceSettingsACL.Edit)]
        public RedirectToRouteResult Edit_POST(EcommerceSettings ecommerceSettings)
        {
            _ecommerceSettingsAdminService.SaveSettings(ecommerceSettings);
            TempData.SuccessMessages().Add("Ecommerce Settings Saved");
            return RedirectToAction("Edit");
        }
    }
}
