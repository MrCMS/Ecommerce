using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;
using static MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeCustomEnumerations;
using StripeResponse = MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeResponse;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly CartModel _cartModel;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly StripeSettings _stripeSettings;

        public StripePaymentService(StripeSettings stripeSettings, CartModel cartModel,
            IOrderPlacementService orderPlacementService, IStringResourceProvider stringResourceProvider,
            EcommerceSettings ecommerceSettings)
        {
            _cartModel = cartModel;
            _orderPlacementService = orderPlacementService;
            _stringResourceProvider = stringResourceProvider;
            _ecommerceSettings = ecommerceSettings;
            _stripeSettings = stripeSettings;
        }

        public StripePaymentDetailsModel GetPaymentDetailsModel()
        {
            //Create Payment Intent using Stripe Payment Service
            var paymentIntent = CreatePaymentIntent();

            return new StripePaymentDetailsModel
            {
                TotalAmount = _cartModel.TotalToPay,
                PublicKey = _stripeSettings.PublicKey,
                CustomerName = _cartModel.BillingAddress.Name,
                PostalCode = _cartModel.BillingAddress.PostalCode,
                LineOne = _cartModel.BillingAddress.Address1,
                LineTwo = _cartModel.BillingAddress.Address2,
                City = _cartModel.BillingAddress.City,
                Country = _cartModel.BillingAddress.CountryCode,
                State = _cartModel.BillingAddress.StateProvince,
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            };
        }

        private PaymentIntent CreatePaymentIntent()
        {
            //Provide customer private key to Stripe and it is mandatory
            StripeConfiguration.ApiKey = _stripeSettings.PrivateKey;

            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(_cartModel.TotalToPay * 100),
                Currency = _ecommerceSettings.CurrencyCode(),
                Metadata = new Dictionary<string, string>()
                    {{"CartId", _cartModel.CartGuid.ToString()},
                     {"CustomerName", _cartModel.BillingAddress.Name}},
            };

            //Create the PaymentIntent instance
            var paymentIntentService = new PaymentIntentService();

            return paymentIntentService.Create(paymentIntentOptions);
        }

        public StripeResponse GetChargeAttemptOutcome(StripePaymentDetailsModel model)
        {
            if (!model.IsSuccessful)
                return new StripeResponse
                {
                    ErrorMessage = _stringResourceProvider.GetValue("payment-stripe-payment-failed-error",
                        $"Your payment was unsuccessful, please try again."),
                    Success = false
                };

            //get the order from API and update cart status
            var chargesList = GetChargeAttemptsList(model.PaymentIntentId);

            var successfulChargeResult = chargesList.FirstOrDefault(c => c.Status == CardPaymentStatus.succeeded.ToString() &&
                                                                    c.Amount == _cartModel.TotalToPay * 100); //total pay in pennies

            // The current successful charge found. Hence, create a MrCMS Order
            if (successfulChargeResult != null)
            {
                return PlaceOrder(successfulChargeResult);
            }

            // The current successful charge is not found.
            return new StripeResponse
            {
                Success = false,
                ErrorMessage = _stringResourceProvider.GetValue("payment-stripe-payment-failed-incorrect-value",
                    $"No payment can be found for {_cartModel.TotalToPay.ToCurrencyFormat()}.")
            };
        }

        // Use Stripe.js to get a list of recent Charge Attempts associated with one Payment Intent.
        // The returned list is sorted chronologically and in a decreasing order. the first success case, 
        // is always the last attempt in succeeding charge attempt. If no success entry found, 
        // all list elements are attempted charges with status other than success
        public StripeList<Charge> GetChargeAttemptsList(string paymentIntentId)
        {
            var service = new ChargeService();

            var options = new ChargeListOptions
            {
                PaymentIntent = paymentIntentId,
                Limit = 10, //the default is 10. charge attempt events are sorted in a descending chronological order               
            };

            var charges = service.List(options);

            return charges;
        }

        public StripeResponse PlaceOrder(Charge currentCharge)
        {
            if (currentCharge == null)
            {
                throw new Exception("Stripe charge must be valid to place order.");
            }

            //Create MrCMS Order object
            try
            {
                var order = _orderPlacementService.PlaceOrder(_cartModel,
                    o =>
                    {
                        o.PaymentStatus = PaymentStatus.Paid;
                        o.CaptureTransactionId = currentCharge.Id;
                        o.PaymentMethod = _cartModel.PaymentMethod.Name;
                    });

                // Success
                return new StripeResponse
                {
                    Success = true,
                    Order = order
                };
            }
            catch (Exception ex)
            {
                // error return      
                return new StripeResponse
                {
                    Success = false,
                    ErrorMessage = "Exception encountered while trying to charge your card. Description: " + ex.Message
                };
            }
        }
    }
}