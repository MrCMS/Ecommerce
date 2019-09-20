using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripeBalanceTransaction : BalanceTransaction
    {
        public Entities.Orders.Order Order { get; set; }
        public int OrderId { get { return this.Order.Id; } }
    }
}