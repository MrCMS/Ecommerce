using System.Web.Mvc;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class OrderInfoController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IAmazonOrderService _amazonOrderService;

        public OrderInfoController(IAmazonOrderService amazonOrderService)
        {
            _amazonOrderService = amazonOrderService;
        }

        public ActionResult AmazonInfo(Order order)
        {
            if (order != null)
                return PartialView(_amazonOrderService.GetByOrderId(order.Id));
            return new EmptyResult();
        }
    }
}