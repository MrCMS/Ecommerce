using System;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.CustomerFeedback.Settings;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.ModelBinders
{
    public class CustomerFeedbackSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public CustomerFeedbackSettingsModelBinder(IKernel kernel, IConfigurationProvider configurationProvider) : base(kernel)
        {
            _configurationProvider = configurationProvider;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return _configurationProvider.GetSiteSettings<CustomerFeedbackSettings>();
        }
    }
}