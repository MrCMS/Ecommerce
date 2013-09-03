using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Logs;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonDashboardModel
    {
        public IPagedList<AmazonLog> Logs { get; set; }
        public int NoOfOrdersInLast7Days { get; set; }
        public int NoOfPublishedListings { get; set; }
        public int NoOfApiCallsToday { get; set; }
    }
}