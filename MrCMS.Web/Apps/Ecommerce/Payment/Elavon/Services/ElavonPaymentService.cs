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
using MrCMS.Services.Resources;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Services
{
    public class ElavonPaymentService : IElavonPaymentService
    {
        private readonly ElavonSettings _elavonSettings;
        private readonly CartModel _cartModel;
        private readonly IOrderPlacementService _orderPlacementService;
        private EcommerceSettings _ecommerceSettings;
        private Country[] isoCountriesList;
        private IStringResourceProvider _stringResourceProvider;

        public ElavonPaymentService(ElavonSettings elavonSettings, CartModel cartModel, EcommerceSettings ecommerceSettings,
                                    ICartBuilder cartBuilder, ISession session, IOrderPlacementService orderPlacementService,
                                    ILogAdminService logAdminService, IUniquePageService uniquePageService,
                                    IStringResourceProvider stringResourceProvider)
        {
            _elavonSettings = elavonSettings;
            _cartModel = cartModel;
            _ecommerceSettings = ecommerceSettings;
            _orderPlacementService = orderPlacementService;
            isoCountriesList = ISO3166.Country.List;
            _stringResourceProvider = stringResourceProvider;
        }

        public ElavonPaymentDetailsModel GetElavonPaymentDetailsModel()
        {
            return new ElavonPaymentDetailsModel()
            {
                ServiceUrl = _elavonSettings.ServiceUrl
            };
        }

        public string GetPaymentRequestResult()
        {
            string chargeRequestResult;
            string requestResult;

            var hppJson = BuildChargeRequest(out chargeRequestResult);

            if (chargeRequestResult != string.Empty) //error happened
            {
                requestResult = "An error  encountered: " + chargeRequestResult;
            }
            else
            {
                requestResult = hppJson;
            }

            return requestResult;
        }
        private string BuildChargeRequest(out string chargeRequestResult)
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

                //double check why I added this
                chargeRequestResult = string.Empty;
            }
            catch (ApiException exce)
            {
                // For example if the SHA1HASH doesn't match what is expected
                chargeRequestResult = "Api call exception happened. Description: " + exce.Message;
            }

            return hppResult;
        }

        public ElavonCustomResult CheckNotificationResult(string responseJson, out bool isSuccessNotification)
        {
            var elavonCustomResult = HandleNotification(responseJson);
            isSuccessNotification = false;

            switch (elavonCustomResult.ElavonResultType)
            {
                case ElavonCustomEnumerations.ResultType.ChargeSuccess:
                    var succeededOrder = elavonCustomResult.ElavonResponse.Order;
                    isSuccessNotification = true;
                    break;
                case ElavonCustomEnumerations.ResultType.TamperedTotalPay:

                    var incorrectChargeResourceName = "payment-elavon-charge-incorrect-value";
                    var incorrectChargeResourcePrefix = "Charge incorrect value: ";
                    elavonCustomResult.ErrorMessageResource = GetErrorMessageResource(incorrectChargeResourceName, incorrectChargeResourcePrefix, elavonCustomResult);
                    break;

                case ElavonCustomEnumerations.ResultType.ChargeFailure:
                    var chargeDeclinedResourceName = "payment-elavon-charge-request-declined";
                    var chargeDeclinedResourcePrefix = "Charge request declined: ";
                    elavonCustomResult.ErrorMessageResource = GetErrorMessageResource(chargeDeclinedResourceName, chargeDeclinedResourcePrefix, elavonCustomResult);
                    break;

                case ElavonCustomEnumerations.ResultType.TamperedHashException:
                    var tamperedHashResourceName = "payment-elavon-charge-tampered-hash-exception";
                    var tamperedHashResourcePrefix = "Tampered hash exception: ";
                    elavonCustomResult.ErrorMessageResource = GetErrorMessageResource(tamperedHashResourceName, tamperedHashResourcePrefix, elavonCustomResult);
                    break;

                case ElavonCustomEnumerations.ResultType.CommsException:
                    var commsExceptionResourceName = "payment-elavon-gateway-exception";
                    var commsExceptionResourcePrefix = "Payment gateway exception: ";
                    elavonCustomResult.ErrorMessageResource = GetErrorMessageResource(commsExceptionResourceName, commsExceptionResourcePrefix, elavonCustomResult);
                    break;
            }

            return elavonCustomResult;
        }

        private string GetErrorMessageResource(string resourceName, string errorMsgPrefix, ElavonCustomResult elavonResult)
        {
            return GetUpdatedErrorMessage(resourceName, errorMsgPrefix + elavonResult.ElavonResponse.ErrorDescription);
        }
        private ElavonCustomResult HandleNotification(string responseJson)
        {
            ElavonCustomResult elavonCustomResult = new ElavonCustomResult();
            elavonCustomResult.ExceptionDescription = string.Empty;

            // Create HPP response object, which contains all the transaction response values 
            // we need to update our application
            // configure client settings
            var service = new HostedService(new GatewayConfig
            {
                AccountId = _elavonSettings.AccountId,
                SharedSecret = _elavonSettings.SharedSecret,
                MerchantId = _elavonSettings.MerchantId,
                ServiceUrl = _elavonSettings.ServiceUrl,
            });

            // Check if there was communication error
            string commsErrorMessage;

            if (HasCommsException(responseJson, out commsErrorMessage))
            {
                elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.CommsException;
                elavonCustomResult.ElavonResponse = new ElavonResponse
                {
                    Success = false,
                    ErrorDescription = commsErrorMessage
                };
            }
            else
            {
                try
                {
                    // create the response object from the response JSON
                    Transaction transaction = service.ParseResponse(responseJson, true);

                    // Total pay in pens
                    var adjustedTotalPay = Math.Round(_cartModel.TotalToPay, 2, MidpointRounding.AwayFromZero) * 100;
                    var transactionBalanceAmount = transaction.AuthorizedAmount;
                    bool transactionResponseCodeIsOk = transaction.ResponseCode.Equals("00");

                    if (transactionResponseCodeIsOk)
                    {
                        if (transactionBalanceAmount == adjustedTotalPay)
                        {
                            ElavonResponse elavonResponse = PlaceOrder(transaction);
                            elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.ChargeSuccess;
                            elavonCustomResult.ElavonResponse = elavonResponse;
                        }
                        else
                        {
                            elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.TamperedTotalPay;
                            elavonCustomResult.ElavonResponse = new ElavonResponse
                            {
                                Success = false,
                                ErrorDescription = string.Format("No payment can be found for a total amount of {0}. Total pay is tampered with.", adjustedTotalPay)
                            };
                        }
                    }
                    else
                    {
                        elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.ChargeFailure;
                        elavonCustomResult.ElavonResponse = new ElavonResponse
                        {
                            Success = false,
                            ErrorDescription = transaction.ResponseMessage
                        };
                    }
                }
                catch (ApiException exce)
                {
                    // Invalid or tampered hash
                    elavonCustomResult.ElavonResultType = ElavonCustomEnumerations.ResultType.TamperedHashException;
                    elavonCustomResult.ExceptionDescription = exce.Message;
                    elavonCustomResult.ElavonResponse = null;
                }
            }

            return elavonCustomResult;
        }

        public ElavonResponse PlaceOrder(Transaction currentTransaction)
        {
            //try creating MrCMS Order object
            try
            {
                if (currentTransaction != null)
                {
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
                        ErrorDescription = string.Empty
                    };
                }
                else
                {
                    // error return      
                    return new ElavonResponse
                    {
                        Success = false,
                        ErrorDescription = "No transaction found"
                    };
                }
            }
            catch (Exception ex)
            {
                // error return      
                return new ElavonResponse
                {
                    Success = false,
                    ErrorDescription = "Exception encountered while trying to charge your card. Description: " + ex.Message
                };
            }
        }

        private string GetUpdatedErrorMessage(string errorMsgName, string errorMessage)
        {
            //get the error message from resource if one exists
            var errorMsgResource = _stringResourceProvider.AllResources.FirstOrDefault(r => r.Key.Equals(errorMsgName));

            if (errorMsgResource != null)
            {
                //update it with the custom error message
                errorMsgResource.Value = errorMessage;
            }
            else
            {
                errorMsgResource = new MrCMS.Entities.Resources.StringResource() { Key = errorMsgName, Value = errorMessage };
                _stringResourceProvider.Insert(errorMsgResource);
            }

            return errorMsgResource.Value;
        }

        private bool HasCommsException(string responseJson, out string commsErrorMessage)
        {
            // The field containing the JSON response, i.e. hppResponse 
            // Because the response includes html markup(i.e <BR>), the use of .Unvalidated() is
            // required. Otherwise, it throws a potentially dangerous request error
            var response = responseJson;
            bool commsException = false;
            string[] messageDetails = new string[2];

            commsErrorMessage = string.Empty;

            //Error encountered before payment is processed on Elavon service
            if (responseJson.Contains("<BR>") || responseJson.Contains("<br>"))
            {
                messageDetails = responseJson.Replace("<BR>", ",").Split(',');
                commsErrorMessage = messageDetails[1].Contains("COMMS ERROR") ? "Payment was not successful due to communication error. Please try again." : messageDetails[1];
                commsException = true;
            }

            return commsException;
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
            else if (transactionDeclined)
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
                //Normal or Non-3d Secured config
               /*  AccountId = _elavonSettings.AccountId,
                SharedSecret = _elavonSettings.SharedSecret,
                MerchantId = _elavonSettings.MerchantId,
                ServiceUrl = _elavonSettings.ServiceUrl,
                HostedPaymentConfig = new HostedPaymentConfig
                {
                    Version = "2"
                }
                //End Normal config
                */
                // 3D Secure config
                
                 AccountId = "ecom3ds",    
                 SharedSecret = "secret",      
                 MerchantId = "myMerchantId",  
                 ServiceUrl =  "https://api.sandbox.realexpayments.com/epage-remote.cgi",  
                 HostedPaymentConfig = new HostedPaymentConfig
                 {
                     Version = "2"
                 },
                 //3D Secure mandatory fields - viz. ThreeDSecure Method & Challenge Notifications
                 Secure3dVersion = Secure3dVersion.Two,
                 MethodNotificationUrl = "Apps/Ecommerce/Elavon/ThreeDSecureMethodNotification",
                 ChallengeNotificationUrl = "Apps/Ecommerce/Elavon/ThreeDSecureChallengeNotification"
             
                // End 3D Secure config

            });

            return service;
        }

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
            var billingAddressCountry = isoCountriesList.FirstOrDefault(c => c.TwoLetterCode.Equals(_cartModel.BillingAddress.CountryCode));

            var billingAddress = new GlobalPayments.Api.Entities.Address
            {
                StreetAddress1 = _cartModel.BillingAddress.Address1,
                StreetAddress2 = _cartModel.BillingAddress.Address2,
                StreetAddress3 = _cartModel.BillingAddress.Address2,
                City = _cartModel.BillingAddress.City,
                PostalCode = _cartModel.BillingAddress.PostalCode,
                Country = billingAddressCountry != null ? billingAddressCountry.NumericCode : string.Empty
            };

            return billingAddress;
        }
        private GlobalPayments.Api.Entities.Address BuildShippingAddress()
        {
            var shippingAddressCountry = isoCountriesList.FirstOrDefault(c => c.TwoLetterCode.Equals(_cartModel.BillingAddress.CountryCode));

            var shippingAddress = new GlobalPayments.Api.Entities.Address
            {
                StreetAddress1 = _cartModel.ShippingAddress.Address1,
                StreetAddress2 = _cartModel.ShippingAddress.Address2,
                StreetAddress3 = _cartModel.ShippingAddress.Address2,
                City = _cartModel.ShippingAddress.City,
                PostalCode = _cartModel.ShippingAddress.PostalCode,
                Country = shippingAddressCountry != null ? shippingAddressCountry.NumericCode : string.Empty
            };

            return shippingAddress;
        }
    }
}