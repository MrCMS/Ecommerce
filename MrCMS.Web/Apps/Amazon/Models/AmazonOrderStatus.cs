using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonOrderStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Unshipped")]
        Unshipped,
        [Description("Partially Shipped")]
        PartiallyShipped,
        [Description("Shipped")]
        Shipped,
        [Description("Canceled")]
        Canceled,
        [Description("Unfulfillable")]
        Unfulfillable
    }
}