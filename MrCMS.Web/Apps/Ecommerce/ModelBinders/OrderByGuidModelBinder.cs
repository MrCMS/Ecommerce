using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class OrderByGuidModelBinder : MrCMSDefaultModelBinder
    {
        public OrderByGuidModelBinder(IKernel kernel) : base(kernel)
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