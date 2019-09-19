using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using Stripe;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services
{
    public interface IStripePaymentService
    {
        ChargeCreateOptions ChargeCreateOptions(string token, decimal totalAmount, string customerName);
        MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeResponse MakePayment(ChargeCreateOptions options);
        PaymentIntent CreatePaymentIntent(decimal totalAmount);
        StripeList<Charge> GetChargeAttemptesList(string paymentIntentId);
        Payment.Stripe.Models.StripeResponse BuildMrCMSOrder(Charge charge);
    }
}