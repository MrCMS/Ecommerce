using System;
using System.Collections.Generic;
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
        public virtual AmazonPaymentMethod? PaymentMethod { get; set; }

        public virtual IList<AmazonOrderItem> Items { get; set; }
    }
}