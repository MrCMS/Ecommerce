using System;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public class AmazonOrderSearchService : IAmazonOrderSearchService
    {
        private readonly ISession _session;

        public AmazonOrderSearchService(ISession session)
        {
            _session = session;
        }

        public IPagedList<AmazonOrder> Search(AmazonOrderSearchModel model)
        {
            var queryOver = _session.QueryOver<AmazonOrder>();

            if (!String.IsNullOrWhiteSpace(model.SearchText))
                queryOver = queryOver.Where(o => o.AmazonOrderId.IsInsensitiveLike(model.SearchText, MatchMode.Anywhere) 
                    || o.BuyerName.IsInsensitiveLike(model.SearchText, MatchMode.Anywhere) 
                    || o.BuyerEmail.IsInsensitiveLike(model.SearchText, MatchMode.Anywhere));

            if (model.DateFrom.HasValue)
                queryOver = queryOver.Where(o => o.PurchaseDate > model.DateFrom);

            if (model.DateTo.HasValue)
                queryOver = queryOver.Where(o => o.PurchaseDate < model.DateTo);

            if (model.ShippingStatus.HasValue)
                queryOver = queryOver.Where(o => o.Status == model.ShippingStatus.GetEnumByValue<AmazonOrderStatus>());

            return queryOver.OrderBy(o => o.Id).Desc.Paged(model.Page);
        }
    }
}