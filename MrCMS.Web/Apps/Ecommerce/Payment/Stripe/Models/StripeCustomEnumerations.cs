using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripeCustomEnumerations
    {
        public enum ResultType
        {
            ChargeSuccess,
            ChargeFailure,
            ChargeNotFound,
            BadRequest
        }

        //For exhaustive list of status refer stripe dashboard 
        public enum CardPaymentStatus
        {
            succeeded,
            failed,
            incomplete,
            uncaptured
        }        
    }
}