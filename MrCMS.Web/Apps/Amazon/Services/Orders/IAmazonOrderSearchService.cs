using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderSearchService
    {
        IPagedList<AmazonOrder> Search(AmazonOrderSearchModel model);
    }
}