using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum ShippingStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Unshipped")]
        Unshipped = 1,
        [Description("Partially Shipped")]
        PartiallyShipped = 2,
        [Description("Shipped")]
        Shipped = 3,
        [Description("Canceled")]
        Canceled = 4,
        [Description("Unfulfillable")]
        Unfulfillable = 5,
        [Description("Invoice Unconfirmed")]
        InvoiceUnconfirmed = 6,
    }
}