using MrCMS.ACL;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ElavonSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ElavonSettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [MrCMSACLRule(typeof(ElavonSettingsACL), ElavonSettingsACL.View)]
        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<ElavonSettings>());
        }

        [HttpPost]
        [MrCMSACLRule(typeof (ElavonSettingsACL), ElavonSettingsACL.View)]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(ElavonSettingsModelBinder))] ElavonSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

    }

    public class ElavonSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ElavonSettingsModelBinder(IKernel kernel, IConfigurationProvider configurationProvider)
            : base(kernel)
        {
            _configurationProvider = configurationProvider;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return _configurationProvider.GetSiteSettings<ElavonSettings>();
        }
    }

    public class ElavonSettingsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "Elavon Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>{ View };
        }
    }
}