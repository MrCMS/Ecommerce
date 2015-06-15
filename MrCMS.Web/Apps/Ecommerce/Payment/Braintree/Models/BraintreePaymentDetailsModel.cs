using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Models
{
    public class BraintreePaymentDetailsModel
    {
        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string CardholderName { get; set; }

        [Required]
        public string CVV { get; set; }

        [Required]
        public int ExpirationMonth { get; set; }

        [Required]
        public int ExpirationYear { get; set; }

        public string PostalCode { get; set; }

        public decimal TotalToPay { get; set; }
    }
}