using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class Sales
    {
        public DateTime Date { get; set; }
        public int OrdersCount { get; set; }
        public int OrderItemsCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}