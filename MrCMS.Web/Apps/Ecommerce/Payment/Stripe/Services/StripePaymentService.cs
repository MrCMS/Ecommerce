using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using StripeResponse = MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeResponse;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly CartModel _cartModel;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly ILogAdminService _logAdminService;
        private readonly ICartBuilder _cartBuilder;
        private readonly ISession _session;
        //Signing secret copied from the endpoint config section of the dashboard
        const string secret = "whsec_DdBQwdfwo03wV5VnvzEnxhoO3aNssySe";
        private ViewDataDictionary _ViewDataDictionary;
        private string _ViewName;
        private string _stripeBalanceTransactionId;
        private IUniquePageService _uniquePageService;
        public StripePaymentService(StripeSettings stripeSettings, CartModel cartModel, ICartBuilder cartBuilder,
                                    ISession session, IOrderPlacementService orderPlacementService, ILogAdminService logAdminService,
                                    IUniquePageService uniquePageService)
        {
            _stripeSettings = stripeSettings;
            _cartModel = cartModel;
            _orderPlacementService = orderPlacementService;
            _logAdminService = logAdminService;
            _cartBuilder = cartBuilder;
            _session = session;
            _uniquePageService = uniquePageService;
        }
        
        public PaymentIntent CreatePaymentIntent(decimal totalAmount)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_test_HSfKyVKUpA7tADbscgmX9d0w00scE9qsh1";

            var paymentIntentService = new PaymentIntentService();

            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalAmount * 100),
                Currency = "gbp"
            };

            //Create the PaymentIntent instance
            PaymentIntent paymentIntent = paymentIntentService.Create(paymentIntentOptions);

            return paymentIntent;
        }       


        public ActionResult HandleNotification(HttpRequestBase request)
        {
            StripeCustomResult stripeCustomResult = new StripeCustomResult();

            try
            {
                var json = new StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd();
                Event stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], secret);
                Charge charge = null;

                switch (stripeEvent.Type)
                {
                    case "charge.succeeded":

                        charge = (Charge)stripeEvent.Data.Object;
                        CartModel cart = _cartModel;

                        //update the order payment status        
                        //StripeResponse
                        if(charge != null)
                        {
                            stripeCustomResult.StripeResultType = StripeCustomEnumerations.ResultType.ChargeSuccess;
                        }
                        else
                        {
                            stripeCustomResult.StripeResultType = StripeCustomEnumerations.ResultType.ChargeNotFound;
                        }

                        break;

                    case "charge.failed":
                        stripeCustomResult.StripeResultType = StripeCustomEnumerations.ResultType.ChargeFailure;

                        break;

                    default:
                        // Handle other event types

                        break;
                }

                stripeCustomResult.StripeResultType = StripeCustomEnumerations.ResultType.BadRequest;

            }
            catch (StripeException e)
            {
                var testStripeExceptionStop = e.Message;
                // Invalid Signature
                stripeCustomResult.StripeResultType = StripeCustomEnumerations.ResultType.BadRequest;

                return stripeCustomResult;
            }

            return stripeCustomResult;
        }

        public StripeResponse BuildMrCMSOrder(Charge currentCharge)
            {
            //Create Stripe transaction
            StripeBalanceTransaction balanceTransaction = new StripeBalanceTransaction
            {
                    Id = currentCharge.Id,
                    Amount = (long)(_cartModel.TotalToPay * 100)
             };

                //try creating MrCMS Order object
                try
                {
                    if (currentCharge != null)
                    {
                        var testTwoStop = string.Empty;

                        Entities.Orders.Order order = _orderPlacementService.PlaceOrder(_cartModel,
                            o =>
                            {
                                o.PaymentStatus = PaymentStatus.Paid;
                                o.CaptureTransactionId = balanceTransaction.Id;
                                o.Total = currentCharge.Amount;
                                o.Subtotal = currentCharge.Amount;
                                o.TotalPaid = currentCharge.Amount;
                            });

                    //set the current stripe order id
                    balanceTransaction.Order = order;
                    _stripeBalanceTransactionId = balanceTransaction.Id;          

                    // Success
                    return new StripeResponse
                        {
                            Success = true,
                            Order = order,
                            Errors = new System.Collections.Generic.List<string>()
                        };
                    }
                    else
                    {
                        // error return      
                        return new StripeResponse
                        {
                            Success = false,
                            Errors = new List<string> { currentCharge.FailureMessage }
                        };
                    }

                }
                catch (Exception ex)
                {
                    // error return      
                    return new StripeResponse
                    {
                        Success = false,
                        Errors = new List<string> { "Exception encountered while trying to charge your card. Descrtiption: " + ex.Message }
                    };
                }

            }                     
              
         public StripeList<Charge> GetChargeAttemptesList(string paymentIntentId)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_test_HSfKyVKUpA7tADbscgmX9d0w00scE9qsh1";

            var service = new ChargeService();

            var options = new ChargeListOptions
            {
                PaymentIntentId = paymentIntentId, //"{{PAYMENT_INTENT_ID}}",
                // Limit the number of objects to return (the default is 10)
                Limit = 3,
            };

            var charges = service.List(options);

            return charges;
        }       

        /*
        public StripeResponse MakePayment(ChargeCreateOptions options)
        {
            StripeGateway stripeGateway = GetGateway();

            //Stripe Charge Service
            var service = new ChargeService();
            var chargeCreateOptions = options;

            try
            {
                Charge charge = service.Create(chargeCreateOptions);

                if((bool)charge.Captured)
                {
                    Entities.Orders.Order order = _orderPlacementService.PlaceOrder(_cartModel,
                        o =>
                        {
                            o.PaymentStatus = PaymentStatus.Paid;
                            o.CaptureTransactionId = charge.BalanceTransactionId;
                        });

                    // Success
                    return new StripeResponse { Success = true, Order = order };
                }
                else
                {
                    // error return      
                    return new StripeResponse
                    {
                        Success = false,
                        Errors = new List<string> { charge.FailureMessage }
                    };
                }

            }
            catch(Exception ex)
            {
                // error return      
                return new StripeResponse
                {
                    Success = false,
                    Errors = new List<string> { "Exception encountered while trying to charge your card. Descrtiption: "+ ex.Message }
                };
            }          

        }

        public ChargeCreateOptions ChargeCreateOptions(string token, decimal totalAmount, string customerName)
        {
            var testStop = string.Empty;

            var optionsDetail = new ChargeCreateOptions
            {
                //Adjust the amount so that Stripe makes proper amount
                Amount = (long)(totalAmount*100),
                Currency = "gbp",
                Description = "Example charge",
                Source = token
            };

            return optionsDetail;
        }
                
    */

        private StripeGateway GetGateway()
        {
            return new StripeGateway
            {
                MerchantId = _stripeSettings.MerchantId,
                PublicKey = _stripeSettings.PublicKey,
                PrivateKey = _stripeSettings.PrivateKey
            };                                  
        }   
        private CartModel GetCart(string orderId)
        {
            string serializedTxCode = JsonConvert.SerializeObject(orderId);
            SessionData sessionData = _session.QueryOver<SessionData>()
                                              .Where(data => data.Key == CartManager.CurrentCartGuid && data.Data == serializedTxCode)
                                              .SingleOrDefault();
            if (sessionData != null)
            {
                CurrentRequestData.UserGuid = sessionData.UserGuid;
                return _cartBuilder.BuildCart(sessionData.UserGuid);
            }
            return null;
        }
    }
}