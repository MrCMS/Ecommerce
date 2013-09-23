using System.ComponentModel;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using System;
namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class OrderSearchModel
    {
        public string Email { get; set; }
        public string LastName { get; set; }
        public string OrderId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        [DisplayName("Payment Status")]
        public PaymentStatus PaymentStatus { get; set; }
        [DisplayName("Shipping Status")]
        public ShippingStatus ShippingStatus { get; set; }
        public IPagedList<Order> Results { get; set; }
    }
}