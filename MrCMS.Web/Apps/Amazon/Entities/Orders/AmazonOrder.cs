using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Entities.Payment;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Amazon.Entities.Orders
{
    public class AmazonOrder : SiteEntity
    {
        public AmazonOrder()
        {
            Payments=new List<AmazonPayment>();
        }

        public virtual Order Order { get; set; }
        public virtual string AmazonOrderId { get; set; }
        public virtual DateTime? PurchaseDate { get; set; }
        public virtual DateTime? LastUpdatedDate { get; set; }
        public virtual string OrderStatus { get; set; }
        public virtual string OrderType { get; set; }
        public virtual string MarketplaceId { get; set; }
        public virtual string SalesChannel { get; set; }

        public virtual string BuyerName { get; set; }
        public virtual string BuyerEmail { get; set; }

        public virtual string ShipServiceLevel { get; set; }
        public virtual string FulfillmentChannel { get; set; }
        public virtual int NumberOfItemsShipped { get; set; }
        public virtual int NumberOfItemsUnshipped { get; set; }
        public virtual Address ShippingAddress { get; set; }
        public virtual string ShipmentServiceLevelCategory { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual string PaymentMethod { get; set; }

        public virtual List<AmazonPayment> Payments { get; set; }
    }
}