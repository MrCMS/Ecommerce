using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EmailAndLoginModel
    {
        [Required]
        public string OrderEmail { get; set; }
        public bool HavePassword { get; set; }
        [Required]
        public string Password { get; set; }
    }
}