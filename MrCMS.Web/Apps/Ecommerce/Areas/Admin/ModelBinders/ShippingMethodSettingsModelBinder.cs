using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class ShippingMethodSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private const string Prefix = "method-";

        public ShippingMethodSettingsModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var shippingMethodSettings = Kernel.Get<ShippingMethodSettings>();

            var request = controllerContext.HttpContext.Request;
            IEnumerable<string> methods = request.Params.AllKeys.Where(key => key.StartsWith(Prefix));
            foreach (var method in methods)
            {
                shippingMethodSettings.EnabledMethods[method.Substring(Prefix.Length)] = request[method].Contains(
                    "true", StringComparison.OrdinalIgnoreCase);
            }

            return shippingMethodSettings;
        }
    }
}