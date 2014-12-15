using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class OrderData
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public decimal OrderTotal { get; set; }

        public int BillingAddressId { get; set; }
        public int? ShippingAddressId { get; set; }
        public int CustomerId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public ShippingStatus ShippingStatus { get; set; }

        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }

        public decimal OrderSubTotalDiscountInclTax { get; set; }

        public decimal OrderSubTotalDiscountExclTax { get; set; }

        public decimal OrderShippingInclTax { get; set; }

        public decimal OrderShippingExclTax { get; set; }

        public decimal OrderTax { get; set; }

        public decimal OrderDiscount { get; set; }

        public bool RewardPointsWereAdded { get; set; }

        public string Email { get; set; }

        public string CustomerIp { get; set; }

        public HashSet<OrderNoteData> Notes { get; set; }

        public DateTime? PaidDate { get; set; }

        public string ShippingMethodName { get; set; }

        public string PaymentMethod { get; set; }
    }
}