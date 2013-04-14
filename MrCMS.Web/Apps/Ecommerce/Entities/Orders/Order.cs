using MrCMS.Entities;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using System;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Orders
{
    public class Order : SiteEntity
    {
        public Order()
        {
            OrderNotes = new List<OrderNote>();
        }

        public virtual decimal Subtotal { get; set; }
        public virtual decimal Tax { get; set; }
        public virtual decimal Total { get; set; }

        public virtual Discount Discount { get; set; }
        public virtual string DiscountCode { get; set; }

        public virtual decimal Weight { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }
        public virtual decimal? ShippingTotal { get; set; }
        public virtual ShippingStatus ShippingStatus { get; set; }

        public virtual Address ShippingAddress { get; set; }
        public virtual Address BillingAddress { get; set; }

        public virtual string PaymentMethod { get; set; }
        public virtual PaymentStatus PaymentStatus { get; set; }
        public virtual DateTime? PaidDate { get; set; }

        public virtual string CustomerIP { get; set; }
        public virtual User User { get; set; }
        public virtual string OrderEmail { get; set; }

        public virtual IList<OrderNote> OrderNotes { get; set; }
    }
}