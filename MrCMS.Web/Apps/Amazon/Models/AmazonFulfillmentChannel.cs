using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonFulfillmentChannel
    {
        [Description("Amazon Fulfillment")]
        AFN,
        [Description("Merchant Fulfillment")]
        MFN
    }
}