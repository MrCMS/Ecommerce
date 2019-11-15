using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using Stripe;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services
{
    public interface IStripePaymentService
    {
        StripePaymentDetailsModel GetPaymentDetailsModel();
        Models.StripeResponse GetChargeAttemptOutcome(StripePaymentDetailsModel model);
    }
}