using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum Gender
    {
        [Description("Male")]
        Male,
        [Description("Female")]
        Female,
        [Description("Unisex")]
        Unisex
    }
}