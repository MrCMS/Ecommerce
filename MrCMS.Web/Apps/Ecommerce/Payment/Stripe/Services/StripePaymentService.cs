using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Areas.Admin.Services;
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
        private readonly EcommerceSettings _ecommerceSettings;

        //Signing secret copied from the endpoint config section of the Stripe dashboard
        private readonly string _stripeSigningSecret;

        public StripePaymentService(StripeSettings stripeSettings, CartModel cartModel, ICartBuilder cartBuilder,
                                    ISession session, IOrderPlacementService orderPlacementService, ILogAdminService logAdminService,
                                    EcommerceSettings ecommerceSettings)                                   
        {
            _stripeSettings = stripeSettings;
            _cartModel = cartModel;
            _orderPlacementService = orderPlacementService;
            _logAdminService = logAdminService;
            _cartBuilder = cartBuilder;
            _session = session;
            StripeConfiguration.ApiKey = stripeSettings.PrivateKey;
            _stripeSigningSecret = stripeSettings.WebhookSigningSecret;
            _ecommerceSettings = ecommerceSettings;
        }

        public PaymentIntent CreatePaymentIntent(decimal totalAmount)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalAmount * 100),
                Currency = _ecommerceSettings.CurrencyCode()
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

                Event stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], _stripeSigningSecret);

                Charge charge = null;

                switch (stripeEvent.Type)
                {
                    case "charge.succeeded":

                        charge = (Charge)stripeEvent.Data.Object;

                        CartModel cart = _cartModel;

                        // Update the order payment status        
                        if (charge != null)
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
                            stripeCustomResult.StripeResultType = StripeCustomEnumerations.ResultType.BadRequest;

                        break;
                }               
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
            // Try creating MrCMS Order object
            try
            {
                if (currentCharge != null)
                {
                    Entities.Orders.Order order = _orderPlacementService.PlaceOrder(_cartModel,
                        o =>
                        {
                            o.PaymentStatus = PaymentStatus.Paid;
                            o.CaptureTransactionId = currentCharge.Id;
                        });

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

        // Use Stripe.js to get a list of recent Charge Attempts associated with one Payment Intent.
        // The returned list is sorted chronologically and in a decreasing order. the first succcess case, 
        // is always the last attempt in succeeding charge attemp. If no success entry found, 
        // all list elements are attempted charges with status other than success
        public StripeList<Charge> GetChargeAttemptesList(string paymentIntentId)
        {
            var service = new ChargeService();
            var options = new ChargeListOptions
            {
                PaymentIntentId = paymentIntentId,
                Limit = 3, //the default is 10. charge attempt events are sorted descending chronological order               
            };

            var charges = service.List(options);
            return charges;
        }
    }
}