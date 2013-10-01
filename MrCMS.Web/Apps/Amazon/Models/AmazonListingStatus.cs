using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonListingStatus
    {
        [Description("Not on Amazon")]
        NotOnAmazon,
        [Description("Active")]
        Active,
        [Description("Inactive (Out of stock)")]
        InactiveOutOfStock,
        [Description("Inactive")]
        Inactive
    }
}