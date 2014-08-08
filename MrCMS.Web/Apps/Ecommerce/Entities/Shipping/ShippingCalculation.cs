using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Shipping
{
    public enum ShippingCriteria
    {
        [Description("Based on cart weight")]
        ByWeight = 1,
        [Description("Based on cart total")]
        ByCartTotal = 2
    }
}