using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using Stripe;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services
{
    public interface IStripePaymentService
    {
        ChargeCreateOptions CreateChargeOptions(string token);
        MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeResponse MakePayment(ChargeCreateOptions options);
    }
}