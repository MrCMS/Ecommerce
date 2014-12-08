using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class AddDiscountApplicationModelBinder : MrCMSDefaultModelBinder
    {
        public AddDiscountApplicationModelBinder(IKernel kernel) : base(kernel)
        {
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type modelType)
        {
            string type = controllerContext.GetValueFromRequest("ApplicationType");
            return Activator.CreateInstance(TypeHelper.GetTypeByName(type));
        }
    }
}