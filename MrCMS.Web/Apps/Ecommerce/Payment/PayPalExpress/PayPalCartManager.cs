﻿using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalCartManager : IPayPalCartManager
    {
        private readonly ICartManager _cartManager;

        public PayPalCartManager(ICartManager cartManager)
        {
            _cartManager = cartManager;
        }

        public void UpdateCart(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            if (!string.IsNullOrWhiteSpace(details.BuyerMarketingEmail))
            {
                _cartManager.SetOrderEmail(details.BuyerMarketingEmail);
            }
            _cartManager.SetPaymentMethod(new PayPalExpressCheckoutPaymentMethod().SystemName);
            _cartManager.SetPayPalExpressInfo(details.Token,
                                              details.PayerInfo.PayerID);

            _cartManager.SetBillingAddress(details.BillingAddress.GetAddress());
            _cartManager.SetCountry(details.BillingAddress.GetCountry());

            var paymentDetails = details.PaymentDetails.FirstOrDefault();
            if (paymentDetails != null)
                _cartManager.SetShippingAddress(paymentDetails.ShipToAddress.GetAddress());
        }
    }
}