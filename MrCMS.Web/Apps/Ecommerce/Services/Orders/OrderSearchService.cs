using System;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using NHibernate.Criterion;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderSearchService : IOrderSearchService
    {
        private readonly ISession _session;

        public OrderSearchService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Order> SearchOrders(OrderSearchModel model)
        {
            var queryOver = _session.QueryOver<Order>();

            if (!String.IsNullOrWhiteSpace(model.SearchText))
                queryOver = queryOver.Where(o => o.OrderEmail.IsInsensitiveLike(model.SearchText, MatchMode.Anywhere) 
                    || o.BillingAddress.LastName.IsInsensitiveLike(model.SearchText, MatchMode.Anywhere));

            if (model.DateFrom.HasValue)
                queryOver = queryOver.Where(o => (o.OrderDate != null && o.OrderDate > model.DateFrom) || (o.OrderDate == null && o.CreatedOn > model.DateFrom.Value));

            if (model.DateTo.HasValue)
                queryOver = queryOver.Where(o => (o.OrderDate != null && o.OrderDate < model.DateTo) || (o.OrderDate == null && o.CreatedOn < model.DateTo.Value));

            if (!String.IsNullOrWhiteSpace(model.OrderId))
                queryOver = queryOver.Where(o => o.Id == Convert.ToInt32(model.OrderId));

            if (model.PaymentStatus.HasValue)
                queryOver = queryOver.Where(o => o.PaymentStatus == model.PaymentStatus);

            if (model.ShippingStatus.HasValue)
                queryOver = queryOver.Where(o => o.ShippingStatus == model.ShippingStatus);

            if (!String.IsNullOrWhiteSpace(model.SalesChannel))
                queryOver = queryOver.Where(o => o.SalesChannel == model.SalesChannel);

            if (model.OrderTotalFrom.HasValue)
                queryOver = queryOver.Where(x => x.Total > model.OrderTotalFrom);

            if (model.OrderTotalTo.HasValue)
                queryOver = queryOver.Where(x => x.Total < model.OrderTotalTo);

            return queryOver.OrderBy(order => order.Id).Desc.Paged(model.Page);
        }
    }
}