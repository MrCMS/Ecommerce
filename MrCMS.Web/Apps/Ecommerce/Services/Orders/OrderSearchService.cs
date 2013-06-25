using System;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderSearchService : IOrderSearchService
    {
        private readonly ISession _session;
        private readonly ISearcher<Order, OrderSearchIndex> _orderSearcher;
        private readonly CurrentSite _currentSite;

        public OrderSearchService(ISession session, CurrentSite currentSite, ISearcher<Order, OrderSearchIndex> orderSearcher)
        {
            _session = session;
            _currentSite = currentSite;
            _orderSearcher = orderSearcher;
        }
        public IPagedList<Order> SearchOrders(string email, string lastname, string orderid, DateTime datefrom, DateTime dateto,
            PaymentStatus paymentStatus = PaymentStatus.Pending, ShippingStatus shippingStatus = ShippingStatus.Pending, int page = 1, int pageSize = 10)
        {
            var searchQuery = new OrderSearchQuery(email,lastname,orderid,datefrom,dateto,paymentStatus,shippingStatus);
            if(datefrom!=dateto)
                return _orderSearcher.Search(searchQuery.GetQuery(), page, pageSize,searchQuery.GetFilter());
            else
                return _orderSearcher.Search(searchQuery.GetQuery(), page, pageSize, null);
        }
    }
}