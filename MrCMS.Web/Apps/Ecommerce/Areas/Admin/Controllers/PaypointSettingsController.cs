using System;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class PaypointSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public PaypointSettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<PaypointSettings>());
        }

        [HttpPost]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(PaypointSettingsModelBinder))] PaypointSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

        private class PaypointSettingsModelBinder : MrCMSDefaultModelBinder
        {
            private readonly IConfigurationProvider _configurationProvider;

            public PaypointSettingsModelBinder(ISession session, IConfigurationProvider configurationProvider)
                : base(() => session)
            {
                _configurationProvider = configurationProvider;
            }

            protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
            {
                return _configurationProvider.GetSiteSettings<PaypointSettings>();
            }
        }
    }

    public class SagePaySettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public SagePaySettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<SagePaySettings>());
        }

        [HttpPost]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(SagePaySettingsModelBinder))] SagePaySettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

        private class SagePaySettingsModelBinder : MrCMSDefaultModelBinder
        {
            private readonly IConfigurationProvider _configurationProvider;

            public SagePaySettingsModelBinder(ISession session, IConfigurationProvider configurationProvider)
                : base(() => session)
            {
                _configurationProvider = configurationProvider;
            }

            protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
            {
                return _configurationProvider.GetSiteSettings<SagePaySettings>();
            }
        }
    }
}