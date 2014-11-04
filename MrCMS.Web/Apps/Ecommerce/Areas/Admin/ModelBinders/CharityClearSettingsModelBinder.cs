using System;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Payment.CharityClear;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    internal class CharityClearSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public CharityClearSettingsModelBinder(IKernel kernel, IConfigurationProvider configurationProvider)
            : base(kernel)
        {
            _configurationProvider = configurationProvider;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return _configurationProvider.GetSiteSettings<CharityClearSettings>();
        }
    }
}