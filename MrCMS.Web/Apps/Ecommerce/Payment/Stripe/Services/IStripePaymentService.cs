using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using Stripe;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services
{
    public interface IStripePaymentService
    {
        PaymentIntent CreatePaymentIntent(decimal totalAmount);
        StripeList<Charge> GetChargeAttemptesList(string paymentIntentId);
        Payment.Stripe.Models.StripeResponse BuildMrCMSOrder(Charge charge);
        ActionResult HandleNotification(HttpRequestBase request);
    }
}