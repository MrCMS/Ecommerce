using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class AddDiscountLimitationModelBinder : MrCMSDefaultModelBinder
    {
        public AddDiscountLimitationModelBinder(IKernel kernel) : base(kernel)
        {
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type modelType)
        {
            string type = controllerContext.GetValueFromRequest("LimitationType");
            return Activator.CreateInstance(TypeHelper.GetTypeByName(type));
        }
    }
}