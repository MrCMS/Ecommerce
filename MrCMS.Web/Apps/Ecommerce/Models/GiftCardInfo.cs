using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers.Validation;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class GiftCardInfo : IGiftCardUserDetails
    {
        [StringLength(100)]
        [DisplayName("Sender Name")]
        [Required]
        public string SenderName { get; set; }

        [StringLength(100)]
        [DisplayName("Sender Email")]
        [EmailValidator]
        [Required]
        public string SenderEmail { get; set; }

        [StringLength(100)]
        [DisplayName("Recipient Name")]
        [Required]
        public string RecipientName { get; set; }

        [StringLength(100)]
        [DisplayName("Recipient Email")]
        [EmailValidator]
        [Required]
        public string RecipientEmail { get; set; }

        [StringLength(1000)]
        [DisplayName("Message")]
        public string Message { get; set; }
    }
}