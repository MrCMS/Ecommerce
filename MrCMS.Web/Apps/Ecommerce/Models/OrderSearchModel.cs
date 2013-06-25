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
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public IPagedList<Order> Results { get; set; }
    }
}