using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class AddDiscountLimitationModelBinder : MrCMSDefaultModelBinder
    {
        public AddDiscountLimitationModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string modelTypeName = controllerContext.Controller.ValueProvider.GetValue("LimitationOpt").AttemptedValue;
            Type type =
                bindingContext.ModelType.Assembly.GetTypes()
                    .SingleOrDefault(x => x.IsSubclassOf(bindingContext.ModelType) && x.FullName == modelTypeName);

            if (type != null)
            {
                bindingContext.ModelMetadata =
                    ModelMetadataProviders.Current.GetMetadataForType(
                        () => CreateModel(controllerContext, bindingContext, type), type);

                var discountLimitation = base.BindModel(controllerContext, bindingContext) as DiscountLimitation;
                discountLimitation.Id = 0;
                return discountLimitation;
            }
            return null;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type modelType)
        {
            object obj = Activator.CreateInstance(modelType);
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, modelType);
            bindingContext.ModelMetadata.Model = obj;
            return obj;
        }
    }
}