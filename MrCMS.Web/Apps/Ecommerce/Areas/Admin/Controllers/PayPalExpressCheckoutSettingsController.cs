using System;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class PayPalExpressCheckoutSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalExpressCheckoutSettingsController(IConfigurationProvider configurationProvider, PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _configurationProvider = configurationProvider;
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        [HttpGet]
        public ActionResult View()
        {
            return View(_payPalExpressCheckoutSettings);
        }

        [HttpPost]
        public ActionResult Save([IoCModelBinder(typeof(PayPalExpressCheckoutSettingsModelBinder))] PayPalExpressCheckoutSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("View");
        }
    }

    public class PayPalExpressCheckoutSettingsModelBinder :MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public PayPalExpressCheckoutSettingsModelBinder(ISession session, IConfigurationProvider configurationProvider) : base(()=>session)
        {
            _configurationProvider = configurationProvider;
        }
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return _configurationProvider.GetSiteSettings<PayPalExpressCheckoutSettings>();
        }
    }
}