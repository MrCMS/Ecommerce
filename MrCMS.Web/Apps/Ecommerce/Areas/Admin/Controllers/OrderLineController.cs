using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderLineController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderLineService _orderService;

        public OrderLineController(IOrderLineService orderLineService)
        {
            _orderService = orderLineService;
        }

        [HttpGet]
        public ActionResult Edit(OrderLine orderLine)
        {
            return View(orderLine);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(OrderLine orderLine)
        {
            _orderService.Save(orderLine);
            return RedirectToAction("Edit", "Order", new { id = orderLine.Order.Id });
        }
    }
}