using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Models
{
    public class BraintreeResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public Order Order { get; set; }
    }
}