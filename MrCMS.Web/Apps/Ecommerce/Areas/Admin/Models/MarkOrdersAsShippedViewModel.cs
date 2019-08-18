using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class MarkOrdersAsShippedViewModel
    {
        public MarkOrdersAsShippedViewModel()
        {
            Orders = new List<Order>();
            Page = 1;
        }
        public List<Order> Orders { get; set; }
        public int Page { get; set; }
    }
}