using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentMethodController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;
        private readonly IOrderService _orderService;
        private readonly IPayPalExpressService _payPalExpressService;
        private readonly IPaypointPaymentService _paypointPaymentService;
        private readonly IDocumentService _documentService;

        public PaymentMethodController(CartModel cartModel, IOrderService orderService, IPayPalExpressService payPalExpressService, IPaypointPaymentService paypointPaymentService, IDocumentService documentService)
        {
            _cartModel = cartModel;
            _orderService = orderService;
            _payPalExpressService = payPalExpressService;
            _paypointPaymentService = paypointPaymentService;
            _documentService = documentService;
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
            return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { id = order.Guid }));
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
            return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { id = order.Guid }));
        }

        [HttpGet]
        public PartialViewResult Paypoint()
        {
            ViewData["months"] = _paypointPaymentService.Months();
            ViewData["start-years"] = _paypointPaymentService.StartYears();
            ViewData["expiry-years"] = _paypointPaymentService.ExpiryYears();
            ViewData["card-types"] = _paypointPaymentService.GetCardTypes();
            return PartialView();
        }

        [HttpPost]
        [ActionName("Paypoint")]
        public ActionResult Paypoint_POST(PaypointPaymentDetailsModel model)
        {
            var response = _paypointPaymentService.ProcessDetails(model, Url.Action("Response3DSecure", "Paypoint"));
            if (response.Requires3DSecure)
            {
                TempData["redirect-details"] = response.RedirectDetails;
                return RedirectToAction("Redirect3DSecure", "Paypoint");
            }

            if (response.PaymentSucceeded)
            {
                var order = _orderService.PlaceOrder(_cartModel, o => { });
                _documentService.RedirectTo<OrderPlaced>(new { id = order.Guid });
            }

            TempData["error-details"] = response.FailureDetails;
            return _documentService.RedirectTo<PaymentDetails>();
        }
    }
}