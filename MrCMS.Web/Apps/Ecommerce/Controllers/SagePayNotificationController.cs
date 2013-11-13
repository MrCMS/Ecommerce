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

        public SagePayNotificationController(SagePaySettings sagePaySettings, ISagePayService sagePayService, ISagePayCartLoader sagePayCartLoader)
        {
            _sagePaySettings = sagePaySettings;
            _sagePayService = sagePayService;
            _sagePayCartLoader = sagePayCartLoader;
        }

        public ActionResult Notification(SagePayResponse response)
        {
            if (string.IsNullOrEmpty(response.VendorTxCode))
                return new ErrorResult();

            var cart = _sagePayCartLoader.GetCart(response.VendorTxCode);
            if (cart == null || cart.CartGuid.ToString() != response.VendorTxCode)
            {
                ResetSessionInfo(cart,
                                 new FailureDetails
                                     {
                                         Message =
                                             "There was an error communicating with SagePay. No funds have been transferred. Please try again, and if you continue to have errors please contact support"
                                     });
                return new TransactionNotFoundResult(response.VendorTxCode);
            }

            if (!response.IsSignatureValid(_sagePayService.GetSecurityKey(cart.UserGuid), _sagePaySettings.VendorName))
            {
                ResetSessionInfo(cart,
                                 new FailureDetails
                                     {
                                         Message =
                                             "There was an error communicating with SagePay. No funds have been transferred. Please try again, and if you continue to have errors please contact support"
                                     });
                return new InvalidSignatureResult(response.VendorTxCode);
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
            }
            return new ValidOrderResult(response.VendorTxCode, response);

        }

        private void ResetSessionInfo(CartModel cart, FailureDetails failureDetails)
        {
            _sagePayService.ResetSessionInfo(cart.UserGuid);
            _sagePayService.SetFailureDetails(cart.UserGuid, failureDetails);
        }
    }
}