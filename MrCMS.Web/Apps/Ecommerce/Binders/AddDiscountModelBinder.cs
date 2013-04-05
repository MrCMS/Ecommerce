//using System;
//using System.Linq;
//using System.Web.Mvc;
//using MrCMS.Helpers;
//using MrCMS.Services;
//using NHibernate;
//using MrCMS.Web.Apps.Ecommerce.Binders;
//using MrCMS.Web.Apps.Ecommerce.Services;
//using MrCMS.Web.Apps.Ecommerce.Entities;

//namespace MrCMS.Web.Apps.Ecommerce.Binders
//{
//    public class AddDiscountGetModelBinder : DiscountModelBinder
//    {
//        public AddDiscountGetModelBinder(ISession session, IDiscountManager discountManager) : base(session, discountManager)
//        {
//        }

//        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
//        {
//            var model = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
//            return model;
//        }
//    }

//    public class AddDiscountModelBinder : DiscountModelBinder
//    {
//        public AddDiscountModelBinder(ISession session, IDiscountManager discountManager)
//            : base(session, discountManager)
//        {
//        }

//        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
//        {
//            var type = GetTypeByName(controllerContext);

//            bindingContext.ModelMetadata =
//                ModelMetadataProviders.Current.GetMetadataForType(
//                    () => CreateModel(controllerContext, bindingContext, type), type);

//            var discount = base.BindModel(controllerContext, bindingContext) as Discount;

//            return discount;
//        }

//        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
//        {
//            var type = GetTypeByName(controllerContext);
//            return Activator.CreateInstance(type);
//        }

//        private static Type GetTypeByName(ControllerContext controllerContext)
//        {
//            //string valueFromContext = GetValueFromContext(controllerContext, "DiscountLimitationType");
//            //return DiscountMetadataHelper.GetTypeByName(valueFromContext)
//            //    ?? TypeHelper.MappedClasses.FirstOrDefault(x => x.Name == valueFromContext);
//            return null;
//        }
//    }
//}