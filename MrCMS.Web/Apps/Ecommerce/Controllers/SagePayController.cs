using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results;
using MrCMS.Website.Controllers;
using NHibernate.Util;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SagePayController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly SagePaySettings _sagePaySettings;
        private readonly CartModel _cartModel;
        private readonly ISagePayService _sagePayService;

        public SagePayController(SagePaySettings sagePaySettings, CartModel cartModel, ISagePayService sagePayService)
        {
            _sagePaySettings = sagePaySettings;
            _cartModel = cartModel;
            _sagePayService = sagePayService;
        }

        public ActionResult Failed(string vendorTxCode)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult Success(string vendorTxCode)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult Notification(SagePayResponse response)
        {
            // SagePay should have sent back the order ID
            if (string.IsNullOrEmpty(response.VendorTxCode))
                return new ErrorResult();

            // IF there was no matching order, send a TransactionNotfound error
            if (_cartModel.CartGuid.ToString() != response.VendorTxCode)
                return new TransactionNotFoundResult(response.VendorTxCode);

            // Check if the signature is valid.
            // Note that we need to look up the vendor name from our configuration.
            if (!response.IsSignatureValid(_sagePayService.GetSecurityKey(), _sagePaySettings.VendorName))
                return new InvalidSignatureResult(response.VendorTxCode);

            // All good - tell SagePay it's safe to charge the customer.
            return new ValidOrderResult(_cartModel.CartGuid.ToString(), response);
        }
    }
}