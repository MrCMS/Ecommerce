using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonPaymentMethod
    {
        [Description("Cash on Delivery")]
        COD,
        [Description("CVS")]
        CVS,
        [Description("Other")]
        Other
    }
}