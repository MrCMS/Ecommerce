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
        public IPagedList<Order> SearchOrders(string email, string lastname, string orderid, DateTime datefrom, DateTime dateto,
            PaymentStatus paymentStatus = PaymentStatus.Pending, ShippingStatus shippingStatus = ShippingStatus.Pending, int page = 1, int pageSize = 10)
        {
            var searchQuery = new OrderSearchQuery(email, lastname, orderid, datefrom, dateto, paymentStatus, shippingStatus);
            return _orderSearcher.Search(searchQuery.GetQuery(), page, pageSize, datefrom != dateto ? searchQuery.GetFilter(): null,
                new Sort(new SortField("id", SortField.INT, true)));
        }
    }
}