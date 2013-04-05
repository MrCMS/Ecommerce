using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Binders;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Binders
{
    public class AddDiscountLimitationModelBinder : DiscountLimitationModelBinder
    {
        public AddDiscountLimitationModelBinder(ISession session, IDiscountLimitationService discountLimitationService)
            : base(session, discountLimitationService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelTypeName = controllerContext.Controller.ValueProvider.GetValue("LimitationOpt").AttemptedValue;
            var type = bindingContext.ModelType.Assembly.GetTypes().SingleOrDefault(x => x.IsSubclassOf(bindingContext.ModelType) && x.FullName == modelTypeName);

            if (type != null)
            {
                bindingContext.ModelMetadata =
                    ModelMetadataProviders.Current.GetMetadataForType(
                        () => CreateModel(controllerContext, bindingContext, type), type);

                var discountLimitation = base.BindModel(controllerContext, bindingContext) as DiscountLimitation;
                discountLimitation.Id = 0;
                return discountLimitation;
            }
            else
            {
                return null;
            }
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var obj = Activator.CreateInstance(modelType);
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, modelType);
            bindingContext.ModelMetadata.Model = obj;
            return obj;
        }
    }
}