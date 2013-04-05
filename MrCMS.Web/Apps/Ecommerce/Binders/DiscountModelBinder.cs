//using System.Linq;
//using System.Web.Mvc;
//using MrCMS.Helpers;
//using MrCMS.Services;
//using NHibernate;
//using NHibernate.Criterion;
//using MrCMS.Web.Apps.Ecommerce.Services;
//using MrCMS.Website.Binders;

//namespace MrCMS.Web.Apps.Ecommerce.Binders
//{
//    public abstract class DiscountModelBinder : MrCMSDefaultModelBinder
//    {
//        protected readonly IDiscountManager DiscountManager;

//        protected DiscountModelBinder(ISession session, IDiscountManager discountManager)
//            : base(() => session)
//        {
//            this.DiscountManager = discountManager;
//        }
//    }
//}