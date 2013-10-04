using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderService
    {
        AmazonOrder Get(int id);
        AmazonOrder GetByAmazonOrderId(string id);
        IPagedList<AmazonOrder> Search(string queryTerm = null, int page = 1, int pageSize = 10);
        void Add(AmazonOrder item);
        void Update(AmazonOrder item);
        void Delete(AmazonOrder item);
        void SaveOrUpdate(List<AmazonOrder> orders);
        void MarkAsShipped(AmazonOrder amazonOrder);
    }
}