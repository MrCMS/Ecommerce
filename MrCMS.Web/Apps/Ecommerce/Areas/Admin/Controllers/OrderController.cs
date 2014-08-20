using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderAdminService _orderAdminService;
        private readonly IUserService _userService;

        public OrderController(IOrderAdminService orderAdminService,IUserService userService)
        {
            _orderAdminService = orderAdminService;
            _userService = userService;
        }

        [HttpGet]
        public ViewResult Index(OrderSearchModel model)
        {
            ViewData["shipping-status-options"] = _orderAdminService.GetShippingStatusOptions();
            ViewData["payment-status-options"] = _orderAdminService.GetPaymentStatusOptions();

            ViewData["results"] = _orderAdminService.Search(model);

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Edit(Order order)
        {
            return order != null
                ? (ActionResult) View(order)
                : RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Cancel(Order order)
        {
            return PartialView(order);
        }

        [ActionName("Cancel")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Cancel_POST(Order order)
        {
            _orderAdminService.Cancel(order);
            return RedirectToAction("Edit", "Order", new {id = order.Id});
        }

        [HttpGet]
        public PartialViewResult MarkAsShipped(Order order)
        {
            ViewData["shipping-method-options"] = _orderAdminService.GetShippingMethodOptions();
            return PartialView(order);
        }

        [ActionName("MarkAsShipped")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult MarkAsShipped_POST(Order order)
        {
            _orderAdminService.MarkAsShipped(order);
            return RedirectToAction("Edit", "Order", new {id = order.Id});
        }

        [HttpGet]
        public PartialViewResult MarkAsPaid(Order order)
        {
            return PartialView(order);
        }

        [ActionName("MarkAsPaid")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult MarkAsPaid_POST(Order order)
        {
            _orderAdminService.MarkAsPaid(order);
            return RedirectToAction("Edit", "Order", new {id = order.Id});
        }

        [HttpGet]
        public PartialViewResult MarkAsVoided(Order order)
        {
            return PartialView(order);
        }

        [ActionName("MarkAsVoided")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult MarkAsVoided_POST(Order order)
        {
            _orderAdminService.MarkAsVoided(order);
            return RedirectToAction("Edit", "Order", new {id = order.Id});
        }


        [HttpGet]
        public ActionResult SetTrackingNumber(Order order)
        {
            return View(order);
        }

        [HttpPost]
        [ActionName("SetTrackingNumber")]
        [ForceImmediateLuceneUpdate]
        public ActionResult SetTrackingNumber_POST(Order order)
        {
            _orderAdminService.SetTrackingNumber(order);
            return RedirectToAction("Edit", "Order", new {id = order.Id});
        }

        [HttpGet]
        public ActionResult Delete(Order order)
        {
            return View(order);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        public ActionResult Delete_POST(Order order)
        {
            _orderAdminService.Delete(order);
            return RedirectToAction("Index", "Order");
        }


        [ChildActionOnly]
        public PartialViewResult ForUser(User user)
        {
            var orders = _userService.GetAll<Order>(user);
            return PartialView(orders);
        }
    }
}