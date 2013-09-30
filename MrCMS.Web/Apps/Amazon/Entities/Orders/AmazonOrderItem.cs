using MarketplaceWebServiceFeedsClasses;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Entities.Orders
{
    public class AmazonOrderItem : SiteEntity
    {
        public virtual AmazonOrder AmazonOrder { get; set; }
        public virtual string AmazonOrderItemId { get; set; }

        public virtual string SellerSKU { get; set; }
        public virtual string Title { get; set; }
        public virtual string ASIN { get; set; }
        public virtual AmazonListingCondition Condition { get; set; }
        public virtual AmazonListingCondition ConditionSubtype { get; set; }

        public virtual string ItemPriceCurrency { get; set; }
        public virtual decimal ItemPriceAmount { get; set; }

        public virtual string ItemTaxCurrency { get; set; }
        public virtual decimal ItemTaxAmount { get; set; }

        public virtual string PromotionDiscountCurrency { get; set; }
        public virtual decimal PromotionDiscountAmount { get; set; }

        public virtual string GiftWrapPriceCurrency { get; set; }
        public virtual decimal GiftWrapPriceAmount { get; set; }

        public virtual string GiftWrapTaxCurrency { get; set; }
        public virtual decimal GiftWrapTaxAmount { get; set; }

        public virtual decimal QuantityOrdered { get; set; }
        public virtual decimal QuantityShipped { get; set; }

        public virtual string ShippingPriceCurrency { get; set; }
        public virtual decimal ShippingPriceAmount { get; set; }

        public virtual string ShippingTaxCurrency { get; set; }
        public virtual decimal ShippingTaxAmount { get; set; }

        public virtual string ShippingDiscountCurrency { get; set; }
        public virtual decimal ShippingDiscountAmount { get; set; }

        public virtual OrderAcknowledgementItemCancelReason? CancelReason { get; set; }
    }
}