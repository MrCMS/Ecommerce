using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MrCMS.Entities;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using System;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Orders
{
    public class Order : SiteEntity
    {
        public Order()
        {
            OrderLines = new List<OrderLine>();
            OrderNotes = new List<OrderNote>();
            OrderRefunds = new List<OrderRefund>();
            Guid = Guid.NewGuid();
        }

        public virtual Guid Guid { get; set; }
        public virtual decimal Subtotal { get; set; }

        public virtual decimal Tax { get; set; }
        public virtual decimal Total { get; set; }

        [DisplayName("Total after Refunds")]
        public virtual decimal TotalAfterRefunds
        {
            get
            {
                return Total - TotalRefunds;
            }
        }

        protected virtual decimal TotalRefunds
        {
            get
            {
                return OrderRefunds.Sum(item => item.Amount);
            }
        }

        public virtual decimal DiscountAmount { get; set; }
        public virtual Discount Discount { get; set; }
        [DisplayName("Discount Code")]
        public virtual string DiscountCode { get; set; }

        public virtual decimal Weight { get; set; }
        [DisplayName("Shipping Method")]
        public virtual ShippingMethod ShippingMethod { get; set; }
        [DisplayName("Shipping Total")]
        public virtual decimal? ShippingTotal { get; set; }
        [DisplayName("Shipping Tax")]
        public virtual decimal? ShippingTax { get; set; }
        [DisplayName("Shipping Tax Percentage")]
        public virtual decimal? ShippingTaxPercentage { get; set; }
        [DisplayName("Shipping Status")]
        public virtual ShippingStatus ShippingStatus { get; set; }
        [DisplayName("Shipping Date")]
        public virtual DateTime? ShippingDate { get; set; }

        public virtual Address ShippingAddress { get; set; }
        public virtual Address BillingAddress { get; set; }

        [DisplayName("Payment Method")]
        public virtual string PaymentMethod { get; set; }
        [DisplayName("Payment Status")]
        public virtual PaymentStatus PaymentStatus { get; set; }
        [DisplayName("Payment Date")]
        public virtual DateTime? PaidDate { get; set; }

        [DisplayName("Customer IP")]
        public virtual string CustomerIP { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Order Email")]
        public virtual string OrderEmail { get; set; }

        public virtual IList<OrderLine> OrderLines { get; set; }
        public virtual IList<OrderNote> OrderNotes { get; set; }
        public virtual IList<OrderRefund> OrderRefunds { get; set; }

        public virtual string AuthorisationToken { get; set; }
        public virtual string CaptureTransactionId { get; set; }

        [DisplayName("Is Cancelled")]
        public virtual bool IsCancelled { get; set; }
        
        [DisplayName("Tracking Number")]
        [StringLength(250)]
        public virtual string TrackingNumber { get; set; }

        [DisplayName("Sales Channel")]
        public virtual string SalesChannel { get; set; }


        //Following fields are use for the HTML email sent to users

        public virtual string BillingAddressFormatted
        {
            get { return BillingAddress.GetDescription(); }
        }

        public virtual string ShippingAddressFormatted
        {
            get { return ShippingAddress.GetDescription(); }
        }

        public virtual string ShoppingCartHtml
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("<table cellpadding=2 cellspacing=2 border=0 style='border: 1px solid grey;'>");
                sb.Append("<tr>");
                sb.Append("<td>Product</td>");
                sb.Append("<td>Quantity</td>");
                sb.Append("<td>Item Price</td>");
                sb.Append("<td>Total</td>");
                sb.Append("</tr>");

                foreach (var item in OrderLines)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + item.Name + "</td>");
                    sb.Append("<td>" + item.Quantity + "</td>");
                    sb.Append("<td>" + item.UnitPrice + "</td>");
                    sb.Append("<td>" + item.Subtotal + "</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</table>");
                return sb.ToString();
            }
        }
    }
}