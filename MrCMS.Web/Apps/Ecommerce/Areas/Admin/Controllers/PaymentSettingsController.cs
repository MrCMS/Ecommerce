using System;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class PaymentSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly PaymentSettings _paymentSettings;

        public PaymentSettingsController(IConfigurationProvider configurationProvider, PaymentSettings paymentSettings)
        {
            _configurationProvider = configurationProvider;
            _paymentSettings = paymentSettings;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_paymentSettings);
        }

        [HttpPost]
        public ActionResult Save([IoCModelBinder(typeof(PaymentSettingsModelBinder))] PaymentSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

        private class PaymentSettingsModelBinder : MrCMSDefaultModelBinder
        {
            private readonly IConfigurationProvider _configurationProvider;

            public PaymentSettingsModelBinder(ISession session, IConfigurationProvider configurationProvider)
                : base(() => session)
            {
                _configurationProvider = configurationProvider;
            }
            protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
            {
                return _configurationProvider.GetSiteSettings<PaymentSettings>();
            }
        }
    }
}