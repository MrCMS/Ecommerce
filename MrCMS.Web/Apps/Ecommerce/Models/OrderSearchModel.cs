using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using System;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class OrderSearchModel
    {
        [DisplayName("Email or last name")]
        public string SearchText { get; set; }
        [DisplayName("Order Id")]
        public string OrderId { get; set; }
        [DisplayName("From")]
        public DateTime? DateFrom { get; set; }
        [DisplayName("To")]
        public DateTime? DateTo { get; set; }
        [DisplayName("Payment Status")]
        public PaymentStatus? PaymentStatus { get; set; }
        [DisplayName("Shipping Status")]
        public ShippingStatus? ShippingStatus { get; set; }
        public IPagedList<Order> Results { get; set; }
        [DisplayName("Sales Channel")]
        public string SalesChannel { get; set; }

        public List<SelectListItem> SalesChannelOptions
        {
            get { return EcommerceApp.SalesChannels.BuildSelectItemList(s => s, emptyItemText: "All"); }
        }
    }
}