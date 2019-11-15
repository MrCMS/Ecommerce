using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripeResponse
    {
        public StripeResponse()
        {
            ErrorMessage = string.Empty;
        }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Order Order { get; set; }
    }
}