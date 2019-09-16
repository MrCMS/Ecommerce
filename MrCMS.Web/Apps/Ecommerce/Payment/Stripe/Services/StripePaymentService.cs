using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Areas.Admin.Services;
using Stripe;
using Stripe.Issuing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public StripeResponse MakePayment(ChargeCreateOptions options)
        {
            StripeGateway stripeGateway = GetGateway();

            //Stripe Charge Service
            var service = new ChargeService();

            Charge charge = service.Create(options);          
    
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
                
        // error return       

            return new StripeResponse
            {
                Success = false,
                Errors = new List<string> { charge.FailureMessage }
            };
        }

        public ChargeCreateOptions CreateChargeOptions(string token)
        {
            var optionsDetail = new ChargeCreateOptions
            {
                Amount = 999,
                Currency = "gbp",
                Description = "Example charge",
                Source = token,
            };

            return optionsDetail;
        }

    }
}