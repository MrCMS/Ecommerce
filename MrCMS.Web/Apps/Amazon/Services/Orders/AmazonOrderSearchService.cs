using System;
using Lucene.Net.Search;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Indexing;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public class AmazonOrderSearchService : IAmazonOrderSearchService
    {
        private readonly ISearcher<AmazonOrder, AmazonOrderSearchIndex> _orderSearcher;

        public AmazonOrderSearchService(ISearcher<AmazonOrder, AmazonOrderSearchIndex> orderSearcher)
        {
            _orderSearcher = orderSearcher;
        }
        public IPagedList<AmazonOrder> Search(string email, string lastname, string orderid, DateTime datefrom, DateTime dateto,
            ShippingStatus shippingStatus = ShippingStatus.Pending, int page = 1, int pageSize = 10)
        {
            var searchQuery = new AmazonOrderSearchQuery(email, lastname, orderid, datefrom, dateto, shippingStatus);
            return _orderSearcher.Search(searchQuery.GetQuery(), page, pageSize, datefrom != dateto ? searchQuery.GetFilter() : null, new Sort(new SortField("id", SortField.INT, true)));
        }
    }
}