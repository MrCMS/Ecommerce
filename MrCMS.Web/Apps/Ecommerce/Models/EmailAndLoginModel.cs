using System.ComponentModel.DataAnnotations;
using MrCMS.Models.Auth;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EmailAndLoginModel :LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,4}\.[0-9]{1,4}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid email.")]
        public string OrderEmail { get; set; }
        public bool HavePassword { get; set; }
    }
}