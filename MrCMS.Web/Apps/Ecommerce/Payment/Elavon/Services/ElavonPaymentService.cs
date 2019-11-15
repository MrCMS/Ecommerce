using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using ISO3166;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models.ElavonCustomEnumerations;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Services
{
    public class ElavonPaymentService : IElavonPaymentService
    {
        private readonly ElavonSettings _elavonSettings;
        private readonly CartModel _cartModel;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly ILogAdminService _logAdminService;
        private readonly ICartBuilder _cartBuilder;
        private readonly ISession _session;
        private IUniquePageService _uniquePageService;
        private EcommerceSettings _ecommerceSettings;
        private Country[] isoCountriesList;

        public ElavonPaymentService(ElavonSettings elavonSettings, CartModel cartModel, EcommerceSettings ecommerceSettings, 
                                    ICartBuilder cartBuilder, ISession session, IOrderPlacementService orderPlacementService, 
                                    ILogAdminService logAdminService, IUniquePageService uniquePageService)
        {
            _elavonSettings = elavonSettings;
            _cartModel = cartModel;
            _ecommerceSettings = ecommerceSettings;
            _orderPlacementService = orderPlacementService;
            _logAdminService = logAdminService;
            _cartBuilder = cartBuilder;
            _session = session;
            _uniquePageService = uniquePageService;
            isoCountriesList = ISO3166.Country.List;
        }
        public string BuildChargeRequest(out string chargeRequestResult)
        {
            var service = ConfigureHostedService();
            var hostedPaymentData = BuildHostedPaymentData();
            var billingAddress = BuildBillingAddress();
            var shippingAddress = BuildShippingAddress();
            string hppResult = string.Empty;

            try
            {
                var totalPay = Math.Round(_cartModel.TotalToPay, 2, MidpointRounding.AwayFromZero);
                var currentCurrency = _ecommerceSettings.CurrencyCode();

                hppResult = service.Charge(totalPay)
                                   .WithCurrency(currentCurrency)
                                   .WithHostedPaymentData(hostedPaymentData)
                                   .WithAddress(billingAddress, AddressType.Billing)
                                   .WithAddress(shippingAddress, AddressType.Shipping)
                                   .WithOrderId(_cartModel.CartGuid.ToString())
                                   .Serialize();

                chargeRequestResult = string.Empty;
            }
            catch (ApiException exce)
            {
                // For example if the SHA1HASH doesn't match what is expected
                chargeRequestResult = "Api call exception happened. Description: " + exce.Message;
            }

            return hppResult;
        }

        public ActionResult HandleNotification(string responseJson)
        {
            ElavonCustomResult elavonCustomResult = new ElavonCustomResult();

            // Create HPP response object, which contains all the transaction response values 
            // we need to update your application
            // configure client settings
            var service = new HostedService(new GatewayConfig
            {
                AccountId = _elavonSettings.AccountId,
                SharedSecret = _elavonSettings.SharedSecret,
                MerchantId = _elavonSettings.MerchantId,
                ServiceUrl = _elavonSettings.ServiceUrl,
            });

            try
            {
                // create the response object from the response JSON
                var transaction = service.ParseResponse(responseJson, true);

                var responseCode = transaction.ResponseCode;
                var responseMessage = transaction.ResponseMessage;
                var orderId = transaction.OrderId;

                var authCode = transaction.AuthorizationCode;
                var paymentsReference = transaction.TransactionId;
                var schemeReferenceData = transaction.SchemeId;

                var responseValues = transaction.ResponseValues;


                // Total pay in pens
                var adjustedTotalPay = Math.Round(_cartModel.TotalToPay, 2, MidpointRounding.AwayFromZero)*100;

                var transactionBalanceAmount = transaction.AuthorizedAmount;

                bool transactionResponseCodeIsOk = responseCode.Equals("00");

                // TODO: update your application and display transaction outcome to the customer
                if(transactionResponseCodeIsOk && transactionBalanceAmount == adjustedTotalPay)
                {
                    ElavonResponse elavonResponse = BuildMrCMSOrder(transaction);

                    elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.ChargeSuccess;
                    elavonCustomResult.ElavonResponse = elavonResponse;
                }
                else if (transactionBalanceAmount != adjustedTotalPay)
                {
                    elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.TamperedTotalPay;
                    elavonCustomResult.ElavonResponse = null;
                }
                else
                {
                    elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.ChargeFailure;
                    elavonCustomResult.ElavonResponse = null;
                }
                elavonCustomResult.ExceptionDescription = string.Empty;
            }
            catch (ApiException exce)
            {
                // Invalid or tampred hash
                elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.TamperedHashException;
                elavonCustomResult.ExceptionDescription = exce.Message;
            }

            return elavonCustomResult;
        }        
        public ElavonResponse BuildMrCMSOrder(Transaction currentTransaction)
        {
            //try creating MrCMS Order object
            try
            {
                if(currentTransaction != null)
                {
                    var testTwoStop = string.Empty;

                    Entities.Orders.Order order = _orderPlacementService.PlaceOrder(_cartModel,
                    o =>
                    {
                        o.PaymentStatus = MapTransactionStatus(currentTransaction.ResponseMessage);
                        o.CaptureTransactionId = currentTransaction.TransactionId;
                        o.PaymentMethod = _cartModel.PaymentMethod.Name;
                    });

                    // Success
                    return new ElavonResponse
                    {
                        Success = true,
                        Order = order,
                        Errors = new System.Collections.Generic.List<string>()
                    };
                }
                else
                {
                    // error return      
                    return new ElavonResponse
                    {
                        Success = false,
                        Errors = new List<string> { currentTransaction.ResponseMessage }
                    };
                }
            }
            catch(Exception ex)
            {
                // error return      
                return new ElavonResponse
                {
                    Success = false,
                    Errors = new List<string> { "Exception encountered while trying to charge your card. Description: " + ex.Message }
                };
            }
        }      
        
        private PaymentStatus MapTransactionStatus(string responseMessage)
        {
            PaymentStatus paymentStatus = PaymentStatus.Pending;

            var transactionAuthorised = responseMessage.ToLowerInvariant().Equals(PaymentTransactionStatus.Authorised.ToString().ToLowerInvariant());
            var transactionSuccessful = responseMessage.ToLowerInvariant().Equals(PaymentTransactionStatus.Successful.ToString().ToLowerInvariant());
            var transactionDeclined = responseMessage.ToLowerInvariant().Equals(PaymentTransactionStatus.Declined.ToString().ToLowerInvariant());

            if (transactionAuthorised || transactionSuccessful) 
            {
                paymentStatus = PaymentStatus.Paid;
            }
            else if(transactionDeclined)
            {
                paymentStatus = PaymentStatus.Voided;
            }
            else
            {
                paymentStatus = PaymentStatus.Pending;
            }

            return paymentStatus;
        }
        private HostedService ConfigureHostedService()
        {
            var service = new HostedService(new GatewayConfig
            {
                AccountId = _elavonSettings.AccountId,           
                SharedSecret = _elavonSettings.SharedSecret,       
                MerchantId = _elavonSettings.MerchantId,          
                ServiceUrl = _elavonSettings.ServiceUrl,
                HostedPaymentConfig = new HostedPaymentConfig
                {
                    Version = "2"
                } 
            });

            return service;
        }                     

        // Add 3D Secure 2 Mandatory and Recommended Fields
        private HostedPaymentData BuildHostedPaymentData()
        {
            var hostedPaymentData = new HostedPaymentData
            {                
                CustomerEmail = _cartModel.OrderEmail,
                CustomerPhoneMobile = _cartModel.BillingAddress.PhoneNumber,
                AddressesMatch = false,
                AccountHolderName = _cartModel.BillingAddress.Name,
                ChallengeRequest = ChallengeRequestIndicator.NO_PREFERENCE,
                CustomerNumber = _cartModel.UserGuid.ToString()         
            };

            return hostedPaymentData;
        }
        private GlobalPayments.Api.Entities.Address BuildBillingAddress()
        {
            var billingAddressCountry = isoCountriesList.ToList()
                                                        .Where(c => c.TwoLetterCode.Equals(_cartModel.BillingAddress.CountryCode))
                                                        .FirstOrDefault();

            var billingAddress = new GlobalPayments.Api.Entities.Address
            {
                StreetAddress1 = _cartModel.BillingAddress.Address1,
                StreetAddress2 = _cartModel.BillingAddress.Address2,
                StreetAddress3 = _cartModel.BillingAddress.Address2,
                City = _cartModel.BillingAddress.City,
                PostalCode = _cartModel.BillingAddress.PostalCode,
                Country = billingAddressCountry.NumericCode
            };

            return billingAddress;
        }
        private GlobalPayments.Api.Entities.Address BuildShippingAddress()
        {
            var shippingAddressCountry = isoCountriesList.ToList()
                                                         .Where(c => c.TwoLetterCode.Equals(_cartModel.BillingAddress.CountryCode))
                                                         .FirstOrDefault();

            var shippingAddress = new GlobalPayments.Api.Entities.Address
            {
                 StreetAddress1 = _cartModel.ShippingAddress.Address1,
                StreetAddress2 = _cartModel.ShippingAddress.Address2,
                StreetAddress3 = _cartModel.ShippingAddress.Address2,
                City = _cartModel.ShippingAddress.City,
                PostalCode = _cartModel.ShippingAddress.PostalCode,
                Country = shippingAddressCountry.NumericCode
            };

            return shippingAddress;
        }
    }
}