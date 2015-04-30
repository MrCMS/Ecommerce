using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Payment.Braintree;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class BraintreeSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public BraintreeSettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [MrCMSACLRule(typeof(BraintreeSettingsACL), BraintreeSettingsACL.View)]
        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<BraintreeSettings>());
        }

        [HttpPost]
        [MrCMSACLRule(typeof (BraintreeSettingsACL), BraintreeSettingsACL.View)]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(BraintreeSettingsModelBinder))] BraintreeSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

    }

    public class BraintreeSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public BraintreeSettingsModelBinder(IKernel kernel, IConfigurationProvider configurationProvider)
            : base(kernel)
        {
            _configurationProvider = configurationProvider;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return _configurationProvider.GetSiteSettings<BraintreeSettings>();
        }
    }

    public class BraintreeSettingsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "Braintree Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>{ View };
        }
    }
}