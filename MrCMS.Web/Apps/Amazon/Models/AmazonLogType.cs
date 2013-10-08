using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonLogType
    {
        [Description("App")]
        App,
        [Description("Api")]
        Api,
        [Description("Listings")]
        Listings,
        [Description("Listing Groups")]
        ListingGroups,
        [Description("Orders")]
        Orders,
        [Description("App Settings")]
        AppSettings,
        [Description("Seller Settings")]
        SellerSettings,
        [Description("Logs")]
        Logs,
    }
}