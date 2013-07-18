using System;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Payments;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderService _orderService;
        private readonly IShippingStatusService _shippingStatusService;
        private readonly IPaymentStatusService _paymentStatusService;
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly IOrderSearchService _orderSearchService;

        public OrderController(IOrderService orderService, IShippingStatusService shippingStatusService,
            IPaymentStatusService paymentStatusService, IShippingMethodManager shippingMethodManager,
            IOrderSearchService orderSearchService)
        {
            _orderService = orderService;
            _shippingStatusService = shippingStatusService;
            _paymentStatusService = paymentStatusService;
            _shippingMethodManager = shippingMethodManager;
            _orderSearchService = orderSearchService;
        }

        [HttpGet]
        public ViewResult Index(OrderSearchModel model, int page = 1)
        {
            if (model.DateFrom.ToString().Contains("1.1.0001."))
                model.DateFrom = DateTime.Now;
            if (model.DateTo.ToString().Contains("1.1.0001."))
                model.DateTo = DateTime.Now;
            ViewData["ShippingStatuses"] = _shippingStatusService.GetOptions();
            ViewData["PaymentStatuses"] = _paymentStatusService.GetOptions();
            model.Results = new PagedList<Order>(null, 1, 10);
            try
            {
                model.Results = _orderSearchService.SearchOrders(model.Email, model.LastName, model.OrderId, model.DateFrom, model.DateTo, model.PaymentStatus, model.ShippingStatus, page);
            }
            catch (Exception)
            {
                model.Results = _orderService.GetPaged(page);
            }

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Edit(Order order)
        {
            ViewData["ShippingStatuses"] = _shippingStatusService.GetOptions();
            ViewData["PaymentStatuses"] = _paymentStatusService.GetOptions();
            ViewData["ShippingMethods"] = _shippingMethodManager.GetOptions();
            if (order.ShippingMethod != null)
                ViewData["ShippingMethodId"] = order.ShippingMethod.Id;
            return order != null
                       ? (ActionResult)View(order)
                       : RedirectToAction("Index");
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(Order order, int shippingMethodId = 0)
        {
            if (shippingMethodId != 0)
                order.ShippingMethod = _shippingMethodManager.Get(shippingMethodId);
            order.User = CurrentRequestData.CurrentUser;
            _orderService.Save(order);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditShippingStatus(Order order, bool index = false)
        {
            ViewData["ShippingStatuses"] = _shippingStatusService.GetOptions();
            ViewBag.Index = index;
            return order != null
                       ? (ActionResult)View(order)
                       : RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [ActionName("EditShippingStatus")]
        [HttpPost]
        public RedirectToRouteResult EditShippingStatus_POST(Order order, bool index = false)
        {
            order.User = CurrentRequestData.CurrentUser;
            _orderService.Save(order);
            if (!index)
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            else
                return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditPaymentStatus(Order order, bool index = false)
        {
            ViewData["PaymentStatuses"] = _paymentStatusService.GetOptions();
            ViewBag.Index = index;
            return order != null
                       ? (ActionResult)View(order)
                       : RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [ActionName("EditPaymentStatus")]
        [HttpPost]
        public RedirectToRouteResult EditPaymentStatus_POST(Order order, bool index = false)
        {
            order.User = CurrentRequestData.CurrentUser;
            _orderService.Save(order);
            if (!index)
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            else
                return RedirectToAction("Index");
        }
    }
}