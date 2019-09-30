using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models
{
    public class ElavonPaymentDetailsModel
    {
       // [Required]
       public string SharedSecret { get; set; }

      //  [Required]
        public decimal TotalAmount { get; set; }
                
        public string SourceToken { get; set; }

        public string CustomerName { get; set; }

        public bool PaymentIntentStatus { get; set; }

        public string HandleCardPaymentStatus { get; set; }

        public string PaymentIntentId { get; set; }

        public string ServiceUrl { get; set; }


        //Address detail properties
        public string LineOne { get; set; }
        public string LineTwo { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
    }
}