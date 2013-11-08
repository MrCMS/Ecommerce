using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderService
    {
        AmazonOrder Get(int id);
        AmazonOrder GetByOrderId(int id);
        AmazonOrder GetByAmazonOrderId(string id);
        IPagedList<AmazonOrder> Search(string queryTerm = null, int page = 1, int pageSize = 10);
        void Update(AmazonOrder item);
        void SaveOrUpdate(AmazonOrder amazonOrder);
        void SaveOrUpdate(List<AmazonOrder> orders);
        void MarkAsShipped(AmazonOrder amazonOrder);
        
    }
}