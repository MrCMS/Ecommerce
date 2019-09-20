using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeCustomEnumerations;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripeCustomResult : EmptyResult
    {
        public ResultType StripeResultType { get; set; }
    }
}