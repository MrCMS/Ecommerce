using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Orders
{
    public class Order : SiteEntity, IBelongToUser
    {
        public Order()
        {
            OrderLines = new List<OrderLine>();
            OrderNotes = new List<OrderNote>();
            OrderRefunds = new List<OrderRefund>();
            GiftCardUsages = new List<GiftCardUsage>();
            DiscountUsages = new List<DiscountUsage>();
            Guid = Guid.NewGuid();
        }


        public virtual Guid Guid { get; set; }
        public virtual decimal Subtotal { get; set; }

        public virtual decimal Tax { get; set; }
        public virtual decimal Total { get; set; }
        public virtual decimal TotalPaid { get; set; }

        [DisplayName("Total after Refunds")]
        public virtual decimal TotalAfterRefunds
        {
            get { return Total - TotalRefunds; }
        }

        public virtual decimal TotalRefunds
        {
            get { return OrderRefunds.Sum(item => item.Amount); }
        }

        public virtual decimal DiscountAmount { get; set; }


        //[DisplayName("Discount Code")]
        //public virtual string DiscountCode { get; set; }

        public virtual decimal RewardPointsAppliedAmount { get; set; }

        public virtual decimal Weight { get; set; }
        //[DisplayName("Shipping Method")]
        //public virtual ShippingMethod ShippingMethod { get; set; }
        [DisplayName("Shipping Method Name")]
        public virtual string ShippingMethodName { get; set; }

        [DisplayName("Shipping Total")]
        public virtual decimal? ShippingSubtotal { get; set; }

        [DisplayName("Shipping Tax")]
        public virtual decimal? ShippingTax { get; set; }

        public virtual decimal ShippingDiscountAmount { get; set; }

        [DisplayName("Shipping Total")]
        public virtual decimal? ShippingTotal { get; set; }


        [DisplayName("Shipping Tax Percentage")]
        public virtual decimal? ShippingTaxPercentage { get; set; }

        [DisplayName("Shipping Status")]
        public virtual ShippingStatus ShippingStatus { get; set; }

        [DisplayName("Shipping Date")]
        public virtual DateTime? ShippingDate { get; set; }

        public virtual AddressData ShippingAddress { get; set; }
        public virtual AddressData BillingAddress { get; set; }

        [DisplayName("Payment Method")]
        public virtual string PaymentMethod { get; set; }

        [DisplayName("Payment Status")]
        public virtual PaymentStatus PaymentStatus { get; set; }

        [DisplayName("Payment Date")]
        public virtual DateTime? PaidDate { get; set; }

        [DisplayName("Customer IP")]
        public virtual string CustomerIP { get; set; }

        [DisplayName("HTTP Data")]
        public virtual string HttpData { get; set; }

        [DisplayName("Order Email")]
        public virtual string OrderEmail { get; set; }

        public virtual IList<OrderLine> OrderLines { get; set; }
        public virtual IList<OrderNote> OrderNotes { get; set; }
        public virtual IList<OrderRefund> OrderRefunds { get; set; }
        public virtual IList<GiftCardUsage> GiftCardUsages { get; set; }
        public virtual IList<DiscountUsage> DiscountUsages { get; set; }

        public virtual string AuthorisationToken { get; set; }
        public virtual string CaptureTransactionId { get; set; }

        [DisplayName("Is Cancelled")]
        public virtual bool IsCancelled { get; set; }

        [DisplayName("Tracking Number")]
        [StringLength(250)]
        public virtual string TrackingNumber { get; set; }

        [DisplayName("Sales Channel")]
        public virtual string SalesChannel { get; set; }

        [DisplayName("Gift Message")]
        public virtual string GiftMessage { get; set; }

        public virtual OrderStatus OrderStatus
        {
            get
            {
                if (IsCancelled)
                    return OrderStatus.Cancelled;

                if (PaymentStatus == PaymentStatus.Voided)
                    return OrderStatus.Void;

                if (PaymentStatus == PaymentStatus.Paid &&
                    (ShippingStatus == ShippingStatus.Shipped || ShippingStatus == ShippingStatus.ShippingNotRequired))
                    return OrderStatus.Complete;

                if (ShippingStatus == ShippingStatus.Shipped || ShippingStatus == ShippingStatus.ShippingNotRequired)
                    return OrderStatus.Shipped;

                if (PaymentStatus == PaymentStatus.Paid)
                    return OrderStatus.Paid;

                return OrderStatus.Pending;
            }
        }

        public virtual User User { get; set; }

    }

    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Complete,
        Void,
        Cancelled
    }
}