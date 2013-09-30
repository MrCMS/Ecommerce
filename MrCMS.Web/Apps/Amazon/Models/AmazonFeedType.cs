using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonFeedType
    {
        [Description("Product Feed")]
        _POST_PRODUCT_DATA_,
        [Description("Product Pricing Feed")]
        _POST_PRODUCT_PRICING_DATA_,
        [Description("Product Inventory Feed")]
        _POST_INVENTORY_AVAILABILITY_DATA_,
        [Description("Product Image Feed")]
        _POST_PRODUCT_IMAGE_DATA_,
        [Description("Order Fulfillment Feed")]
        _POST_ORDER_FULFILLMENT_DATA_
    }
}
