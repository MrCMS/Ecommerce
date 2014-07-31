using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Paypoint;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaypointController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IPaypointPaymentService _paypointPaymentService;
        private readonly CartModel _cartModel;
        private readonly IOrderService _orderService;
        private readonly IPaypoint3DSecureHelper _paypoint3DSecureHelper;
        private readonly IUniquePageService _uniquePageService;

        public PaypointController(IPaypointPaymentService paypointPaymentService, CartModel cartModel, IOrderService orderService, IPaypoint3DSecureHelper paypoint3DSecureHelper, IUniquePageService uniquePageService)
        {
            _paypointPaymentService = paypointPaymentService;
            _cartModel = cartModel;
            _orderService = orderService;
            _paypoint3DSecureHelper = paypoint3DSecureHelper;
            _uniquePageService = uniquePageService;
        }


        [HttpGet]
        public PartialViewResult Form()
        {
            ViewData["start-months"] = _paypointPaymentService.StartMonths();
            ViewData["start-years"] = _paypointPaymentService.StartYears();
            ViewData["expiry-months"] = _paypointPaymentService.ExpiryMonths();
            ViewData["expiry-years"] = _paypointPaymentService.ExpiryYears();
            ViewData["card-types"] = _paypointPaymentService.GetCardTypes();
            return PartialView(_paypointPaymentService.GetModel());
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Form(PaypointPaymentDetailsModel model)
        {
            _paypointPaymentService.SetModel(model);

            if (!_cartModel.CanPlaceOrder)
            {
                _cartModel.CannotPlaceOrderReasons.ForEach(s => TempData.ErrorMessages().Add(s));
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }

            var response = _paypointPaymentService.ProcessDetails(model, Url.Action("Response3DSecure", "Paypoint", null, Request.Url.Scheme));
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
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = order.Guid });
            }

            TempData["error-details"] = response.FailureDetails;
            TempData["paypoint-model"] = model;
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        [ForceImmediateLuceneUpdate]
        public ActionResult Response3DSecure(FormCollection formCollection)
        {
            if (!_cartModel.CanPlaceOrder)
            {
                TempData["error-details"] = new FailureDetails
                {
                    Message =
                        "We were unable to process your order with the specified cart. Please check your details and try again"
                };
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }
            if (_cartModel.CartGuid != _paypoint3DSecureHelper.GetCartGuid() ||
                _cartModel.Total != _paypoint3DSecureHelper.GetOrderAmount())
            {
                TempData["error-details"] = new FailureDetails
                {
                    Message = "Your order was changed after going off to PayPoint for 3D secure validation. No payment has been taken, but you will need to re-submit your details."
                };
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }

            var response = _paypointPaymentService.Handle3DSecureResponse(formCollection);

            if (response.Requires3DSecure)
            {
                TempData["redirect-details"] = response.RedirectDetails;
                return RedirectToAction("Redirect3DSecure");
            }

            if (response.PaymentSucceeded)
            {
                var order = _orderService.PlaceOrder(_cartModel, o =>
                {
                    o.PaymentStatus = PaymentStatus.Paid;
                    o.ShippingStatus = ShippingStatus.Unshipped;
                    o.AuthorisationToken = response.PaypointPaymentDetails.AuthCode;
                });
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = order.Guid });
            }

            TempData["error-details"] = response.FailureDetails;
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        public ActionResult Redirect3DSecure()
        {
            var redirectDetails = TempData["redirect-details"] as RedirectDetails;
            if (redirectDetails != null)
                return View(redirectDetails);
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }
    }
}