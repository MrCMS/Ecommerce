using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class ShippingMethodModelBinder : MrCMSDefaultModelBinder
    {
        public ShippingMethodModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var method = controllerContext.HttpContext.Request["method"];
            if (string.IsNullOrWhiteSpace(method))
                return null;

            var typeByName = TypeHelper.GetTypeByName(method);
            if (typeByName == null) 
                return null;

            return Kernel.Get(typeByName) as IShippingMethod;
        }
    }
}