using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class OrderSearchModel
    {
        public OrderSearchModel()
        {
            Page = 1;
        }

        public int Page { get; set; }

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

        [DisplayName("Sales Channel")]
        public string SalesChannel { get; set; }

        public List<SelectListItem> SalesChannelOptions
        {
            get { return EcommerceApp.SalesChannels.BuildSelectItemList(s => s, emptyItemText: "All"); }
        }
    }
}