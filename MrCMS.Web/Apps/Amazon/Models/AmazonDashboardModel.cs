using System;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Logs;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonDashboardModel
    {
        public AmazonDashboardModel()
        {
            FilterUntil = DateTime.UtcNow.Date;
            FilterFrom = DateTime.UtcNow.Date.AddDays(-7);
        }

        public IPagedList<AmazonLog> Logs { get; set; }
        public int NoOfOrders { get; set; }
        public int NoOfUnshippedOrders { get; set; }
        public double AverageOrderAmount { get; set; }
        public decimal NoOfOrderedProducts { get; set; }
        public decimal NoOfShippedProducts { get; set; }
        public int NoOfActiveListings { get; set; }
        public int NoOfApiCalls { get; set; }
        public bool AppSettingsStatus { get; set; }
        public bool SellerSettingsStatus { get; set; }

        public DateTime FilterFrom { get; set; }
        public DateTime FilterUntil { get; set; }
    }
}