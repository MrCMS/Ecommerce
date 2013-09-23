using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderService
    {
        AmazonOrder Get(int id);
        AmazonOrder GetByAmazonOrderId(string id);
        IEnumerable<AmazonOrder> GetAll();
        IPagedList<AmazonOrder> Search(string queryTerm = null, int page = 1, int pageSize = 10);
        void Save(AmazonOrder item);
        void Delete(AmazonOrder item);
    }
}