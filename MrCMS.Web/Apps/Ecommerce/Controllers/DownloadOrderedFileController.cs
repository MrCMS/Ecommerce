using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class DownloadOrderedFileController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IDownloadOrderedFileService _downloadOrderedFileService;

        public DownloadOrderedFileController(IDownloadOrderedFileService downloadOrderedFileService)
        {
            _downloadOrderedFileService = downloadOrderedFileService;
        }

        public ActionResult Download([IoCModelBinder(typeof(OrderByGuidModelBinder))] Order order, OrderLine line)
        {
            return (ActionResult)_downloadOrderedFileService.GetDownload(order,line) ?? new EmptyResult();
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
            return Guid.TryParse(GetValueFromContext(controllerContext, "guid") ?? Convert.ToString(controllerContext.RouteData.Values["guid"]), out guid)
                       ? Session.QueryOver<Order>().Where(ord => ord.Guid == guid).Take(1).Cacheable().SingleOrDefault()
                       : null;
        }
    }
}
