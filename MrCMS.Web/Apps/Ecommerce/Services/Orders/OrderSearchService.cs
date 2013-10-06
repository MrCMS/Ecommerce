using System;
using Lucene.Net.Search;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderSearchService : IOrderSearchService
    {
        private readonly ISearcher<Order, OrderSearchIndex> _orderSearcher;

        public OrderSearchService(ISearcher<Order, OrderSearchIndex> orderSearcher)
        {
            _orderSearcher = orderSearcher;
        }
        public IPagedList<Order> SearchOrders(OrderSearchModel model, int page = 1, int pageSize = 10)
        {
            var query = new OrderSearchQuery(model);
            return _orderSearcher.Search(query.GetQuery(), page, pageSize, query.GetFilter(),
                new Sort(new SortField("id", SortField.INT, true)));
        }
    }
}