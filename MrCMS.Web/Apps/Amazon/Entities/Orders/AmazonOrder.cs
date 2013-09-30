using System;
using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Amazon.Entities.Orders
{
    public class AmazonOrder : SiteEntity
    {
        public AmazonOrder()
        {
            Items=new List<AmazonOrderItem>();
        }

        public virtual Order Order { get; set; }
        public virtual string AmazonOrderId { get; set; }
        public virtual DateTime? PurchaseDate { get; set; }
        public virtual DateTime? LastUpdatedDate { get; set; }
        public virtual AmazonOrderStatus? Status { get; set; }
        public virtual string OrderType { get; set; }
        public virtual string MarketplaceId { get; set; }
        public virtual string SalesChannel { get; set; }

        public virtual string BuyerName { get; set; }
        public virtual string BuyerEmail { get; set; }

        public virtual string ShipServiceLevel { get; set; }
        public virtual AmazonFulfillmentChannel? FulfillmentChannel { get; set; }
        public virtual decimal NumberOfItemsShipped { get; set; }
        public virtual decimal NumberOfItemsUnshipped { get; set; }
        public virtual Address ShippingAddress { get; set; }
        public virtual string ShipmentServiceLevelCategory { get; set; }

        public virtual string OrderCurrency { get; set; }
        public virtual decimal OrderTotalAmount { get; set; }

        public virtual decimal ItemDiscountAmount
        {
            get { return Items.Sum(item => item.PromotionDiscountAmount); }
        }

        public virtual decimal ItemAmount
        {
            get { return Items.Sum(item => item.ItemPriceAmount); }
        }

        public virtual decimal ItemTax
        {
            get { return Items.Sum(item => item.ItemTaxAmount); }
        }

        public virtual decimal ShippingTotal
        {
            get { return (ShippingAmount + ShippingTax) - ShippingDiscountAmount; }
        }

        public virtual decimal ShippingAmount
        {
            get { return Items.Sum(item => item.ShippingPriceAmount); }
        }

        public virtual decimal ShippingTax
        {
            get { return Items.Sum(item => item.ShippingTaxAmount); }
        }

        public virtual decimal ShippingDiscountAmount
        {
            get { return Items.Sum(item => item.ShippingDiscountAmount); }
        }

        public virtual decimal GiftWrapAmount
        {
            get { return Items.Sum(item => item.GiftWrapPriceAmount); }
        }

        public virtual decimal GiftWrapTax
        {
            get { return Items.Sum(item => item.GiftWrapTaxAmount); }
        }

        public virtual decimal Tax
        {
            get { return ItemTax + ShippingTax + GiftWrapTax; }
        }

        public virtual AmazonPaymentMethod? PaymentMethod { get; set; }
        public virtual OrderAcknowledgementItemCancelReason? CancelReason { get; set; }
        
        public virtual IList<AmazonOrderItem> Items { get; set; }
    }
}