using System;
using Lucene.Net.Search;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Indexing;
using MrCMS.Web.Apps.Amazon.Models;
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
        
        public IPagedList<AmazonOrder> Search(AmazonOrderSearchModel model, int page = 1, int pageSize = 10)
        {
            var query = new AmazonOrderSearchQuery(model);
            return _orderSearcher.Search(query.GetQuery(), page, pageSize, query.GetFilter(),
                new Sort(new SortField("id", SortField.INT, true)));
        }
    }
}