using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum ProductCondition
    {
        [Description("New")]
        New,
        [Description("Used")]
        Used,
        [Description("Refurbished")]
        Refurbished
    }
}