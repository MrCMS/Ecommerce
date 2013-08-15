using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class PaypointPaymentDetailsModel
    {
        [Required]
        public string CardType { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string CardVerificationCode { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        [Required]
        public int EndMonth { get; set; }
        [Required]
        public int EndYear { get; set; }
        [Required]
        public string NameOnCard { get; set; }
        public string CardIssueNumber { get; set; }
        public bool SignUpForNewsletter { get; set; }
    }
}