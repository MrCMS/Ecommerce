using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonFulfillmentChannel
    {
        [Description("Amazon Fulfillment Network")]
        AFN,
        [Description("Merchant Fulfillment Network")]
        MFN
    }
}