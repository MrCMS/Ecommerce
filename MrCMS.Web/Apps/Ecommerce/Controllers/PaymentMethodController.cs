using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentMethodController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;
        private readonly IOrderService _orderService;
        private readonly IPayPalExpressService _payPalExpressService;

        public PaymentMethodController(CartModel cartModel, IOrderService orderService, IPayPalExpressService payPalExpressService)
        {
            _cartModel = cartModel;
            _orderService = orderService;
            _payPalExpressService = payPalExpressService;
        }

        [HttpGet]
        public PartialViewResult CashOnDelivery()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("CashOnDelivery")]
        public RedirectResult CashOnDelivery_POST()
        {
            var order = _orderService.PlaceOrder(_cartModel, o => { o.PaymentStatus = PaymentStatus.Pending; });
            return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { oid = order.Id }));
        }

        [HttpGet]
        public PartialViewResult PayPalExpressCheckout()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("PayPalExpressCheckout")]
        public RedirectResult PayPalExpressCheckout_POST()
        {
            var response = _payPalExpressService.DoExpressCheckout(_cartModel);
            var order = _orderService.PlaceOrder(_cartModel, response.UpdateOrder);
            return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { oid = order.Id }));
        }
    }
}