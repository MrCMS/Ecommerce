using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public interface IGiftCardUserDetails
    {
        [StringLength(100)]
        [DisplayName("Sender Name")]
        string SenderName { get; set; }

        [StringLength(100)]
        [DisplayName("Sender Email")]
        string SenderEmail { get; set; }

        [StringLength(100)]
        [DisplayName("Recipient Name")]
        string RecipientName { get; set; }

        [StringLength(100)]
        [DisplayName("Recipient Email")]
        string RecipientEmail { get; set; }

        [StringLength(1000)]
        [DisplayName("Message")]
        string Message { get; set; }
    }
}