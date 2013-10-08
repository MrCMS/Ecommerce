using System;
using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ryness.Models
{
    public class CourierDeliverySearchModel
    {
        [DisplayName("From")]
        public DateTime? DateFrom { get; set; }
        [DisplayName("To")]
        public DateTime? DateTo { get; set; }
        public IList<Order> Results { get; set; }
    }
}