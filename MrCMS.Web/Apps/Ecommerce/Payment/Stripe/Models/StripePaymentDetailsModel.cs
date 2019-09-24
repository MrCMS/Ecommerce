using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripePaymentDetailsModel
    {
       // [Required]
       public string PublicKey { get; set; }

      //  [Required]
        public decimal TotalAmount { get; set; }
                
        public string SourceToken { get; set; }

        public string CustomerName { get; set; }

        public bool PaymentIntentStatus { get; set; }

        public string HandleCardPaymentStatus { get; set; }

        public string PaymentIntentId { get; set; }

        //Address detail properties
        public string LineOne { get; set; }
        public string LineTwo { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
    }
}