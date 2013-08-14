using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class RegisterWithoutDetailsModel
    {
        [Required(ErrorMessage = "Email is required")]
        [StringLength(128, MinimumLength = 5)]
        [Remote("CheckEmailIsNotRegistered", "Registration", ErrorMessage = "This email is already registered.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Minimum length for password is {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}