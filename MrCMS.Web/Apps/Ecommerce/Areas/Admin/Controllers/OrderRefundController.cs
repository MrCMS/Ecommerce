using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
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
            OrderRefund orderRefund = new OrderRefund();
            orderRefund.Order = _orderService.Get(orderID);
            orderRefund.Amount = orderRefund.Order.Total;
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
                    orderRefund.Order.OrderRefunds.Add(orderRefund);
                    _orderRefundService.Save(orderRefund);
                    return RedirectToAction("Edit", "Order", new { id = orderRefund.Order.Id });
                }
                else
                {
                    return View(orderRefund);
                }
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
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