using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.CustomerFeedback.Models
{
    public class CustomerInteractionPostModel
    {
        public Order Order { get; set; }
        [Required]
        public string Message { get; set; }
    }
}