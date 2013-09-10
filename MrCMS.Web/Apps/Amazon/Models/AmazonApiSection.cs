using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonApiSection
    {
        [Description("Feeds")]
        Feeds,
        [Description("Reports")]
        Reports,
        [Description("Fulfillment")]
        Fulfillment,
        [Description("Orders")]
        Orders,
        [Description("Sellers")]
        Sellers,
        [Description("Products")]
        Products,
        [Description("Recommendations")]
        Recommendations,
        [Description("Subscriptions")]
        Subscriptions,
        [Description("Off-Amazon Payments")]
        OffAmazonPayments
    }
}