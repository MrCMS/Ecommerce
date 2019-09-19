using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Areas.Admin.Services;
using Stripe;
using System;
using System.Collections.Generic;
using StripeResponse = MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeResponse;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly CartModel _cartModel;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly ILogAdminService _logAdminService;
        public StripePaymentService(StripeSettings stripeSettings, 
                                    CartModel cartModel,
                                    IOrderPlacementService orderPlacementService, 
                                    ILogAdminService logAdminService)
        {
            _stripeSettings = stripeSettings;
            _cartModel = cartModel;
            _orderPlacementService = orderPlacementService;
            _logAdminService = logAdminService;
        }
        private StripeGateway GetGateway()
        {
            return new StripeGateway
            {
                MerchantId = _stripeSettings.MerchantId,
                PublicKey = _stripeSettings.PublicKey,
                PrivateKey = _stripeSettings.PrivateKey
            };                                  
        }

        public StripeResponse BuildMrCMSOrder(Charge currentCharge)
        {        
            //Create Stripe transaction
            BalanceTransaction balanceTransaction = new BalanceTransaction
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

                    Random rand = new Random();
                    int randomOrderId = rand.Next();

                    Entities.Orders.Order order = _orderPlacementService.PlaceOrder(_cartModel,
                        o =>
                        {
                            o.PaymentStatus = PaymentStatus.Pending;
                            o.CaptureTransactionId = balanceTransaction.Id;
                            o.Total = currentCharge.Amount;
                            o.Subtotal = currentCharge.Amount;
                            o.TotalPaid = currentCharge.Amount;
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

        //Payment Intent API 
        public PaymentIntent CreatePaymentIntent(decimal totalAmount)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_test_HSfKyVKUpA7tADbscgmX9d0w00scE9qsh1";

            var service = new PaymentIntentService();

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalAmount * 100),
                Currency = "gbp"
            };

            PaymentIntent paymentIntent = service.Create(options);

            return paymentIntent;
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

            */
        }
}