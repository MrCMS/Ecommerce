using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class AddDiscountModelBinder : MrCMSDefaultModelBinder
    {
        public AddDiscountModelBinder(IKernel kernel) : base(kernel)
        {
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var discountType = controllerContext.GetValueFromRequest("DiscountType");
            var type = TypeHelper.GetTypeByName(discountType);
            return Activator.CreateInstance(type);
        }
    }
}