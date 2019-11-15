namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripePaymentDetailsModel
    {
        public string PublicKey { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public string HandleCardPaymentStatus { get; set; }
        public string PaymentIntentId { get; set; }
        public string LineOne { get; set; }
        public string LineTwo { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public bool IsSuccessful =>
            HandleCardPaymentStatus.ToLowerInvariant()
                .Equals(StripeCustomEnumerations.CardPaymentStatus.succeeded.ToString().ToLowerInvariant());

        public string ClientSecret { get; set; }
    }
}