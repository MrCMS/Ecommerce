using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class PayPalExpressCheckoutSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public PayPalExpressCheckoutSettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(PayPalExpressCheckoutSettingsACL), PayPalExpressCheckoutSettingsACL.View)]
        public ActionResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<PayPalExpressCheckoutSettings>());
        }

        [HttpPost]
        [MrCMSACLRule(typeof(PayPalExpressCheckoutSettingsACL), PayPalExpressCheckoutSettingsACL.View)]
        public RedirectToRouteResult Save(
            [IoCModelBinder(typeof (PayPalExpressCheckoutSettingsModelBinder))] PayPalExpressCheckoutSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }
    }
}