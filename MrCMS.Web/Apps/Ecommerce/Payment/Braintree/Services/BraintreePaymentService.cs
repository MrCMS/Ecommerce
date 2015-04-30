using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Braintree;
using Elmah;
using MrCMS.Logging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Services
{
    public class BraintreePaymentService : IBraintreePaymentService
    {
        private readonly BraintreeSettings _braintreeSettings;
        private readonly CartModel _cartModel;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly ILogAdminService _logAdminService;

        public BraintreePaymentService(BraintreeSettings braintreeSettings, CartModel cartModel, 
            IOrderPlacementService orderPlacementService, ILogAdminService logAdminService)
        {
            _braintreeSettings = braintreeSettings;
            _cartModel = cartModel;
            _orderPlacementService = orderPlacementService;
            _logAdminService = logAdminService;
        }

        //JsonResult
        public string GenerateClientToken()
        {
            var gateway = new BraintreeGateway
            {
                Environment =
                    _braintreeSettings.UseSandbox ? Environment.SANDBOX : Environment.PRODUCTION,
                MerchantId = _braintreeSettings.MerchantId,
                PublicKey = _braintreeSettings.PublicKey,
                PrivateKey = _braintreeSettings.PrivateKey
            };

            var clientToken = gateway.ClientToken.generate();

            return clientToken;
        }

        public BraintreeResponse MakePayment(FormCollection collection)
        {
            string nonce = collection["payment_method_nonce"]; // get nonce
            BraintreeGateway braintreeGateway = GetGateway();
            TransactionRequest request = new TransactionRequest
            {
                Amount = _cartModel.TotalToPay,
                PaymentMethodNonce = nonce
            };

            Result<Transaction> result = braintreeGateway.Transaction.Sale(request); // send transction request

            if (result.IsSuccess())
            {
                Order order = _orderPlacementService.PlaceOrder(_cartModel,
                    o =>
                    {
                        o.PaymentStatus = PaymentStatus.Paid;
                        o.ShippingStatus = _cartModel.RequiresShipping ? ShippingStatus.Unshipped : ShippingStatus.ShippingNotRequired;
                        o.CaptureTransactionId = result.Target.Id;
                    });

                // Success
                return new BraintreeResponse { Success = true, Order = order };
            }

            // error return
            List<string> errorList =
                result.Errors.DeepAll()
                    .Select(
                        error => "Code:" + error.Code + " Message: " + error.Message + " Attribute: " + error.Attribute)
                    .ToList();

            _logAdminService.Insert(new Log
            {
                Error = new Error(),
                Message = "Braintree Error Details",
                Detail = string.Join("<br/>", errorList)
            });

            return new BraintreeResponse
            {
                Success = false,
                Errors = new List<string> { result.Message }
            };
        }

        private BraintreeGateway GetGateway()
        {
            return new BraintreeGateway
            {
                Environment = _braintreeSettings.UseSandbox ? Environment.SANDBOX : Environment.PRODUCTION,
                MerchantId = _braintreeSettings.MerchantId,
                PublicKey = _braintreeSettings.PublicKey,
                PrivateKey = _braintreeSettings.PrivateKey
            };
        }
    }
}