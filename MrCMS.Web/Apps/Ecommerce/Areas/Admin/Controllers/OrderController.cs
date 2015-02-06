using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderAdminService _orderAdminService;

        public OrderController(IOrderAdminService orderAdminService)
        {
            _orderAdminService = orderAdminService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.List)]
        public ViewResult Index(OrderSearchModel model)
        {
            ViewData["shipping-status-options"] = _orderAdminService.GetShippingStatusOptions();
            ViewData["payment-status-options"] = _orderAdminService.GetPaymentStatusOptions();

            ViewData["results"] = _orderAdminService.Search(model);

            return View("Index", model);
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.Edit)]
        public ActionResult Edit(Order order)
        {
            return order != null
                ? (ActionResult)View(order)
                : RedirectToAction("Index");
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.Cancel)]
        public PartialViewResult Cancel(Order order)
        {
            return PartialView(order);
        }

        [ActionName("Cancel")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.Cancel)]
        public RedirectToRouteResult Cancel_POST(Order order)
        {
            _orderAdminService.Cancel(order);
            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.MarkAsShipped)]
        public PartialViewResult MarkAsShipped(Order order)
        {
            ViewData["shipping-method-options"] = _orderAdminService.GetShippingMethodOptions();
            return PartialView(order);
        }

        [ActionName("MarkAsShipped")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.MarkAsShipped)]
        public RedirectToRouteResult MarkAsShipped_POST(Order order)
        {
            _orderAdminService.MarkAsShipped(order);
            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.MarkAsPaid)]
        public PartialViewResult MarkAsPaid(Order order)
        {
            return PartialView(order);
        }

        [ActionName("MarkAsPaid")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.MarkAsPaid)]
        public RedirectToRouteResult MarkAsPaid_POST(Order order)
        {
            _orderAdminService.MarkAsPaid(order);
            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.MarkAsVoided)]
        public PartialViewResult MarkAsVoided(Order order)
        {
            return PartialView(order);
        }

        [ActionName("MarkAsVoided")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.MarkAsVoided)]
        public RedirectToRouteResult MarkAsVoided_POST(Order order)
        {
            _orderAdminService.MarkAsVoided(order);
            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.SetTrackingNumber)]
        public ActionResult SetTrackingNumber(Order order)
        {
            return View(order);
        }

        [HttpPost]
        [ActionName("SetTrackingNumber")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.SetTrackingNumber)]
        public ActionResult SetTrackingNumber_POST(Order order)
        {
            _orderAdminService.SetTrackingNumber(order);
            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.Delete)]
        public ActionResult Delete(Order order)
        {
            return View(order);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(OrderACL), OrderACL.Delete)]
        public ActionResult Delete_POST(Order order)
        {
            _orderAdminService.Delete(order);
            return RedirectToAction("Index", "Order");
        }
    }
}