//using System.Web.Mvc;
//using MrCMS.Entities.Documents;
//using MrCMS.Entities.Documents.Web;
//using MrCMS.Services;
//using NHibernate;
//using MrCMS.Web.Apps.Ecommerce.Services;
//using MrCMS.Web.Apps.Ecommerce.Entities;

//namespace MrCMS.Web.Apps.Ecommerce.Binders
//{
//    public class EditDiscountModelBinder : DiscountModelBinder
//    {
//        public EditDiscountModelBinder(ISession session, IDiscountManager discountManager)
//            : base(session, discountManager)
//        {
//        }

//        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
//        {
//            var discount = base.BindModel(controllerContext, bindingContext) as Discount;

//            return discount;
//        }
//    }
//}