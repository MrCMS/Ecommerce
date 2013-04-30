using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
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

        public OrderController(IOrderService orderService, IShippingStatusService shippingStatusService, IPaymentStatusService paymentStatusService, IShippingMethodManager shippingMethodManager)
        {
            _orderService = orderService;
            _shippingStatusService = shippingStatusService;
            _paymentStatusService = paymentStatusService;
            _shippingMethodManager = shippingMethodManager;
        }

        [HttpGet]
        public ViewResult Index(int page = 1)
        {
            return View(_orderService.GetPaged(page));
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
        public ActionResult EditShippingStatus(Order order)
        {
            ViewData["ShippingStatuses"] = _shippingStatusService.GetOptions();
            ViewData["PaymentStatuses"] = _paymentStatusService.GetOptions();
            return order != null
                       ? (ActionResult)View(order)
                       : RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        [ActionName("EditShippingStatus")]
        [HttpPost]
        public RedirectToRouteResult EditShippingStatus_POST(Order order)
        {
            order.User = CurrentRequestData.CurrentUser;
            _orderService.Save(order);
            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        public ActionResult PlaceNewOrder()
        {
            _orderService.PlaceOrder(new CartModel
                                         {
                                             Items = new List<CartItem>
                                                         {
                                                             new CartItem
                                                                 {
                                                                     Item =
                                                                         MrCMSApplication.Get<IProductService>()
                                                                                         .Search()[0],
                                                                     Quantity = 3
                                                                 }
                                                         },
                                             User = CurrentRequestData.CurrentUser,
                                             ShippingAddress = new Address{},
                                             ShippingMethod = _shippingMethodManager.GetAll()[0]
                                         });
            return RedirectToAction("Index");
        }
    }
}