using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonLogType
    {
        [Description("Listings")]
        Listings,
        [Description("Orders")]
        Orders,
        [Description("Categories")]
        Categories,
        [Description("Api Settings")]
        ApiSettings,
        [Description("Store Settings")]
        StoreSettings,
        [Description("Seller Settings")]
        SellerSettings,
        [Description("Sync Settings")]
        SyncSettings
    }
}