using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class DownloadOrderedFileOrderByGuidModelBinder : MrCMSDefaultModelBinder
    {
        public DownloadOrderedFileOrderByGuidModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Guid guid;
            return Guid.TryParse(GetValueFromContext(controllerContext, "guid") ?? Convert.ToString(controllerContext.RouteData.Values["guid"]), out guid)
                       ? Session.QueryOver<Order>().Where(ord => ord.Guid == guid).Take(1).Cacheable().SingleOrDefault()
                       : null;
        }
    }
    public class OrderByGuidModelBinder : MrCMSDefaultModelBinder
    {
        public OrderByGuidModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Guid guid;
            return Guid.TryParse(GetValueFromContext(controllerContext, "id") ?? Convert.ToString(controllerContext.RouteData.Values["id"]), out guid)
                       ? Session.QueryOver<Order>().Where(ord => ord.Guid == guid).Take(1).Cacheable().SingleOrDefault()
                       : null;
        }
    }
}