using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripeGateway
    {
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}