using System.ComponentModel;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonOrderSearchModel
    {
        public AmazonOrderSearchModel()
        {
            Page = 1;
        }

        public string SearchText { get; set; }
        [DisplayName("Amazon Order #")]
        public string AmazonOrderId { get; set; }
        [DisplayName("From")]
        public DateTime? DateFrom { get; set; }
        [DisplayName("To")]
        public DateTime? DateTo { get; set; }
        [DisplayName("Shipping Status")]
        public ShippingStatus? ShippingStatus { get; set; }
        public IPagedList<AmazonOrder> Results { get; set; }
        public int Page { get; set; }
    }
}