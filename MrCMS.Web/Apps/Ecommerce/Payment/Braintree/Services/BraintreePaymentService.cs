using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Braintree;
using Elmah;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Areas.Admin.Services;
using Environment = Braintree.Environment;

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

        public string GenerateClientToken()
        {
            var gateway = GetGateway();
            var clientToken = gateway.ClientToken.generate();
            return clientToken;
        }

        public BraintreeResponse MakePaymentPaypal(string nonce)
        {
            BraintreeGateway braintreeGateway = GetGateway();
            TransactionRequest request = new TransactionRequest
            {
                Amount = _cartModel.TotalToPay,
                PaymentMethodNonce = nonce,
                BillingAddress = GetBillingAddress(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = braintreeGateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                Order order = _orderPlacementService.PlaceOrder(_cartModel,
                    o =>
                    {
                        o.PaymentStatus = PaymentStatus.Paid;
                        o.CaptureTransactionId = result.Target.Id;
                    });
                return new BraintreeResponse { Success = true, Order = order };
            }
            return new BraintreeResponse
            {
                Success = false,
                Errors = new List<string> { result.Message }
            };
        }

        public BraintreeResponse MakePayment(string nonce)
        {
            BraintreeGateway braintreeGateway = GetGateway();
            TransactionRequest request = new TransactionRequest
            {
                Amount = _cartModel.TotalToPay,
                PaymentMethodNonce = nonce,
                BillingAddress = GetBillingAddress(),
                Options = new TransactionOptionsRequest
                {
                    ThreeDSecure = new TransactionOptionsThreeDSecureRequest
                    {
                        Required = _braintreeSettings.ThreeDSecureRequired
                    }
                }
            };

            Result<Transaction> result = braintreeGateway.Transaction.Sale(request); // send transction request
            
            if (result.IsSuccess())
            {
                Order order = _orderPlacementService.PlaceOrder(_cartModel,
                    o =>
                    {
                        o.PaymentStatus = PaymentStatus.Paid;
                        o.CaptureTransactionId = result.Target.Id;
                    });

                // Success
                return new BraintreeResponse { Success = true, Order = order };
            }

            // error return
            List<string> errorList =
                result.Errors.DeepAll()
                    .Select(
                        error =>
                            string.Format("Code: {0}, Message: {1}, Attribute: {2}", error.Code, error.Message,
                                error.Attribute))
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

        private AddressRequest GetBillingAddress()
        {
            var addressRequest = new AddressRequest
            {
                FirstName = _cartModel.BillingAddress.FirstName,
                LastName = _cartModel.BillingAddress.LastName,
                StreetAddress = _cartModel.BillingAddress.Address1,
                Locality = _cartModel.BillingAddress.City,
                Region = _cartModel.BillingAddress.StateProvince,
                PostalCode = _cartModel.BillingAddress.PostalCode,
                CountryCodeAlpha2 = _cartModel.BillingAddress.CountryCode
            };
            if (!string.IsNullOrWhiteSpace(_cartModel.BillingAddress.Company))
                addressRequest.Company = _cartModel.BillingAddress.Company;
            if (!string.IsNullOrWhiteSpace(_cartModel.BillingAddress.Address2))
                addressRequest.ExtendedAddress = _cartModel.BillingAddress.Address2;

            return addressRequest;
        }

        public IEnumerable<SelectListItem> ExpiryMonths()
        {
            return Enumerable.Range(1, 12).BuildSelectItemList(i => i.ToString().PadLeft(2, '0'), i => i.ToString(), emptyItemText: "Month");
        }

        public IEnumerable<SelectListItem> ExpiryYears()
        {
            return Enumerable.Range(DateTime.Now.Year, 11).BuildSelectItemList(i => i.ToString(), emptyItemText: "Year");
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