using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonLogType
    {
        [Description("Listings")]
        Listings,
        [Description("Orders")]
        Orders,
        [Description("App Settings")]
        AppSettings,
        [Description("Seller Settings")]
        SellerSettings
    }
}