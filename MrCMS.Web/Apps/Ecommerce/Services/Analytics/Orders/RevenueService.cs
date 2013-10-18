using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IRevenueService
    {
        IEnumerable<IGrouping<SalesChannel, Order>> GetBaseDataGroupedBySalesChannel(DateTime from, DateTime to);
        IEnumerable<IGrouping<SalesChannel, Order>> GetBaseDataGroupedBySalesChannel();
    }

    public class RevenueService : IRevenueService
    {
        private readonly ISession _session;

        public RevenueService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<IGrouping<SalesChannel, Order>> GetBaseDataGroupedBySalesChannel(DateTime from, DateTime to)
        {
            return _session.QueryOver<Order>()
                        .Where(item => item.CreatedOn >= from.Date && item.CreatedOn <= to.Date)
                        .Cacheable()
                        .List()
                        .GroupBy(x => x.SalesChannel);
        }

        public IEnumerable<IGrouping<SalesChannel, Order>> GetBaseDataGroupedBySalesChannel()
        {
            return _session.QueryOver<Order>()
                        .Where(item => item.CreatedOn >= CurrentRequestData.Now.Date && item.CreatedOn <= CurrentRequestData.Now.Date.AddHours(24))
                        .Cacheable()
                        .List()
                        .GroupBy(x => x.SalesChannel);
        }
    }
}