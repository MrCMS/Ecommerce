using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models
{
    public class StripePaymentDetailsModel
    {
       // [Required]
        // public string ApiKey { get; set; }

      //  [Required]
        public decimal TotalAmount { get; set; }

       // [Required]
       // public string Currency { get; set; }

      //  [Required]
        //public string Description { get; set; }

      //  [Required]
        public string SourceToken { get; set; }

        public string CustomerName { get; set; }

    }
}