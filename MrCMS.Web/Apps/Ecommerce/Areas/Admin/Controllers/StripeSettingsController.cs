using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class StripeSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public StripeSettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [MrCMSACLRule(typeof(StripeSettingsACL), StripeSettingsACL.View)]
        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<StripeSettings>());
        }

        [HttpPost]
        [MrCMSACLRule(typeof (StripeSettingsACL), StripeSettingsACL.View)]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(StripeSettingsModelBinder))] StripeSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

    }

    public class StripeSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public StripeSettingsModelBinder(IKernel kernel, IConfigurationProvider configurationProvider)
            : base(kernel)
        {
            _configurationProvider = configurationProvider;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return _configurationProvider.GetSiteSettings<StripeSettings>();
        }
    }

    public class StripeSettingsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "Stripe Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>{ View };
        }
    }
}