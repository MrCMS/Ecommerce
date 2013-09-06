using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonListingItemStatus
    {
        [Description("Not Listed")]
        NotListed,
        [Description("Scheduled")]
        Scheduled,
        [Description("Listed")]
        Listed,
        [Description("Sold")]
        Sold,
        [Description("Unsold")]
        Unsold,
        [Description("Returned")]
        Returned
    }
}