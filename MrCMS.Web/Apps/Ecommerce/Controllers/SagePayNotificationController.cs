using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SagePayNotificationController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ISagePayCartLoader _sagePayCartLoader;
        private readonly ISagePayService _sagePayService;
        private readonly SagePaySettings _sagePaySettings;
        private readonly IOrderPlacementService _orderPlacementService;

        public SagePayNotificationController(SagePaySettings sagePaySettings, ISagePayService sagePayService,
            ISagePayCartLoader sagePayCartLoader, IOrderPlacementService orderPlacementService)
        {
            _sagePaySettings = sagePaySettings;
            _sagePayService = sagePayService;
            _sagePayCartLoader = sagePayCartLoader;
            _orderPlacementService = orderPlacementService;
        }

        public ActionResult Notification(SagePayResponse response)
        {
            var vendorTxCode = response.VendorTxCode;
            if (string.IsNullOrEmpty(vendorTxCode))
                return new ErrorResult();
            var cart = _sagePayCartLoader.GetCart(vendorTxCode);
            if (cart == null || cart.CartGuid.ToString() != vendorTxCode ||
                _sagePayService.GetCartTotal(cart.UserGuid) != cart.TotalToPay)
            {
                ResetSessionInfo(cart,
                    new FailureDetails
                    {
                        Message =
                            "There was an error communicating with SagePay. No funds have been transferred. Please try again, and if you continue to have errors please contact support"
                    });
                return new TransactionNotFoundResult(vendorTxCode);
            }

            if (!response.IsSignatureValid(_sagePayService.GetSecurityKey(cart.UserGuid), _sagePaySettings.VendorName))
            {
                ResetSessionInfo(cart,
                    new FailureDetails
                    {
                        Message =
                            "There was an error communicating with SagePay. No funds have been transferred. Please try again, and if you continue to have errors please contact support"
                    });
                return new InvalidSignatureResult(vendorTxCode);
            }

            if (!response.WasTransactionSuccessful)
            {
                ResetSessionInfo(cart, new FailureDetails
                {
                    Message =
                        "SagePay was unable to authorise payment with the provided details. Please confirm they are correct, or try another means of payment"
                });
            }
            else
            {
                _sagePayService.SetResponse(cart.UserGuid, response);
                _orderPlacementService.PlaceOrder(cart, o =>
                {
                    o.PaymentStatus = PaymentStatus.Paid;
                    o.ShippingStatus = ShippingStatus.Unshipped;
                    o.AuthorisationToken = response.BankAuthCode;
                });

            }
            return new ValidOrderResult(vendorTxCode, response);
        }

        private void ResetSessionInfo(CartModel cart, FailureDetails failureDetails)
        {
            if (cart != null)
            {
                _sagePayService.ResetSessionInfo(cart.UserGuid);
                _sagePayService.SetFailureDetails(cart.UserGuid, failureDetails);
            }
        }
    }
}