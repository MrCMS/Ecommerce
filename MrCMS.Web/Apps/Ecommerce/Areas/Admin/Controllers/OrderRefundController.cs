using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderRefundController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderRefundService _orderRefundService;
        private readonly IOrderService _orderService;

        public OrderRefundController(IOrderRefundService orderRefundService, IOrderService orderService)
        {
            _orderRefundService = orderRefundService;
            _orderService = orderService;
        }

        [HttpGet]
        public ViewResult Add(int orderID)
        {
            var orderRefund = new OrderRefund {Order = _orderService.Get(orderID)};
            orderRefund.Amount = orderRefund.Order.TotalAfterRefunds;
            orderRefund.User = CurrentRequestData.CurrentUser;
            return View(orderRefund);
        }

        [ActionName("Add")]
        [HttpPost]
        public ActionResult Add_POST(OrderRefund orderRefund)
        {
            if (orderRefund.Order != null)
            {
                if (orderRefund.Amount > orderRefund.Order.TotalAfterRefunds)
                {
                    ModelState.AddModelError("Amount", "Refund Amount must be equal or less than Total Amount after previous refunds.");
                }
                if (ModelState.IsValid)
                {
                    _orderRefundService.Add(orderRefund);
                    return RedirectToAction("Edit", "Order", new { id = orderRefund.Order.Id });
                }
                return View(orderRefund);
            }
            return RedirectToAction("Index", "Order");
        }

        [HttpGet]
        public PartialViewResult Delete(OrderRefund orderRefund)
        {
            return PartialView(orderRefund);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(OrderRefund orderRefund)
        {
            if (orderRefund.Order != null)
            {
                _orderRefundService.Delete(orderRefund);
                return RedirectToAction("Edit", "Order", new { id = orderRefund.Order.Id });
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }
    }
}