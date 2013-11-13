using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Paypoint;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
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
        private readonly ISagePayService _sagePayService;

        public PaymentMethodController(CartModel cartModel, IOrderService orderService,
            IPayPalExpressService payPalExpressService, IPaypointPaymentService paypointPaymentService,
            IDocumentService documentService, ISagePayService sagePayService)
        {
            _cartModel = cartModel;
            _orderService = orderService;
            _payPalExpressService = payPalExpressService;
            _paypointPaymentService = paypointPaymentService;
            _documentService = documentService;
            _sagePayService = sagePayService;
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
            RedirectResult redirectResult;
            if (!CanPlaceOrder(out redirectResult)) return redirectResult;
            var order = _orderService.PlaceOrder(_cartModel, o => { o.PaymentStatus = PaymentStatus.Pending; });
            return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { id = order.Guid }));
        }

        private bool CanPlaceOrder(out RedirectResult redirectResult)
        {
            redirectResult = null;
            if (!_cartModel.CanPlaceOrder)
            {
                TempData["error-details"] = new FailureDetails
                                                {
                                                    Message =
                                                        "We were unable to process your order with the specified cart. Please check your details and try again"
                                                };
                {
                    redirectResult = _documentService.RedirectTo<PaymentDetails>();
                    return false;
                }
            }
            return true;
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
            RedirectResult redirectResult;
            if (!CanPlaceOrder(out redirectResult)) return redirectResult;

            var response = _payPalExpressService.DoExpressCheckout(_cartModel);

            if (response.Success)
            {
                var order = _orderService.PlaceOrder(_cartModel, response.UpdateOrder);
                return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { id = order.Guid }));
            }

            else
                TempData["error-details"] = new FailureDetails
                                                {
                                                    Message =
                                                        "An error occurred processing your PayPal Express order, please contact the merchant"
                                                };
            return _documentService.RedirectTo<PaymentDetails>();
        }

        [HttpGet]
        public PartialViewResult Paypoint()
        {
            ViewData["start-months"] = _paypointPaymentService.StartMonths();
            ViewData["start-years"] = _paypointPaymentService.StartYears();
            ViewData["expiry-months"] = _paypointPaymentService.ExpiryMonths();
            ViewData["expiry-years"] = _paypointPaymentService.ExpiryYears();
            ViewData["card-types"] = _paypointPaymentService.GetCardTypes();
            return PartialView(_paypointPaymentService.GetModel());
        }

        [HttpPost]
        [ActionName("Paypoint")]
        public ActionResult Paypoint_POST(PaypointPaymentDetailsModel model)
        {
            _paypointPaymentService.SetModel(model);

            RedirectResult redirectResult;
            if (!CanPlaceOrder(out redirectResult)) return redirectResult;

            var response = _paypointPaymentService.ProcessDetails(model,
                                                                  Url.Action("Response3DSecure", "Paypoint", null,
                                                                             Request.Url.Scheme));
            if (response.Requires3DSecure)
            {
                TempData["redirect-details"] = response.RedirectDetails;
                return RedirectToAction("Redirect3DSecure", "Paypoint");
            }

            if (response.PaymentSucceeded)
            {
                var order = _orderService.PlaceOrder(_cartModel, o =>
                                                                     {
                                                                         o.PaymentStatus = PaymentStatus.Paid;
                                                                         o.AuthorisationToken = response.PaypointPaymentDetails.AuthCode;
                                                                         o.ShippingStatus = ShippingStatus.Unshipped;
                                                                     });
                return _documentService.RedirectTo<OrderPlaced>(new { id = order.Guid });
            }

            TempData["error-details"] = response.FailureDetails;
            TempData["paypoint-model"] = model;
            return _documentService.RedirectTo<PaymentDetails>();
        }

        [HttpGet]
        public PartialViewResult SagePay()
        {
            ViewData["error-details"] = _sagePayService.GetFailureDetails(_cartModel.UserGuid);
            var transactionRegistrationResponse = _sagePayService.RegisterTransaction(_cartModel);
            return PartialView(transactionRegistrationResponse);
        }
    }
}