using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EmailAndLoginModel :LoginModel
    {
        [Required]
        public string OrderEmail { get; set; }
        public bool HavePassword { get; set; }
        [Required]
        public string Password { get; set; }
    }
}