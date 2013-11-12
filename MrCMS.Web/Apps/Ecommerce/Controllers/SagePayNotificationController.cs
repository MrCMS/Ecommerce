using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SagePayNotificationController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly SagePaySettings _sagePaySettings;
        private readonly ISagePayService _sagePayService;
        private readonly ISagePayCartLoader _sagePayCartLoader;
        private readonly ICartGuidResetter _cartGuidResetter;

        public SagePayNotificationController(SagePaySettings sagePaySettings, ISagePayService sagePayService, ISagePayCartLoader sagePayCartLoader, ICartGuidResetter cartGuidResetter)
        {
            _sagePaySettings = sagePaySettings;
            _sagePayService = sagePayService;
            _sagePayCartLoader = sagePayCartLoader;
            _cartGuidResetter = cartGuidResetter;
        }

        public ActionResult Notification(SagePayResponse response)
        {
            if (string.IsNullOrEmpty(response.VendorTxCode))
                return new ErrorResult();

            var cart = _sagePayCartLoader.GetCart(response.VendorTxCode);
            if (cart == null || cart.CartGuid.ToString() != response.VendorTxCode)
            {
                ResetCartGuid(cart);
                TempData["message"] =
                    "There was an error communicating with SagePay. No funds have been transferred. Please try again, and if you continue to have errors please contact support";
                return new TransactionNotFoundResult(response.VendorTxCode);
            }

            if (!response.IsSignatureValid(_sagePayService.GetSecurityKey(cart.UserGuid), _sagePaySettings.VendorName))
            {
                ResetCartGuid(cart);
                TempData["message"] =
                    "There was an error communicating with SagePay. No funds have been transferred. Please try again, and if you continue to have errors please contact support";
                return new InvalidSignatureResult(response.VendorTxCode);
            }

            if (!response.WasTransactionSuccessful)
            {
                ResetCartGuid(cart);
                switch (response.Status)
                {
                    case ResponseType.NotAuthed:
                    case ResponseType.Rejected:
                        TempData["error-details"] = new FailureDetails
                                                        {
                                                            Message =
                                                                "SagePay was unable to authorise payment with the provided details. Please confirm they are correct, or try another means of payment"
                                                        };
                        break;
                }
            }
            else
            {
                _sagePayService.SetResponse(cart.UserGuid, response);
            }
            return new ValidOrderResult(response.VendorTxCode, response);

        }

        private void ResetCartGuid(CartModel cart)
        {
            _cartGuidResetter.ResetCartGuid(cart.UserGuid);
        }
    }
}