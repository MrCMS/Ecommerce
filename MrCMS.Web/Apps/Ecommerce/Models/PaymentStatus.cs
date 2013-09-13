using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum PaymentStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Paid")]
        Paid,
        [Description("Partially Refunded")]
        PartiallyRefunded,
        [Description("Refunded")]
        Refunded,
        [Description("Voided")]
        Voided
    }
}