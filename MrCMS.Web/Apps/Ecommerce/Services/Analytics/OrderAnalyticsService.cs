using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics
{
    public interface IOrderAnalyticsService
    {
        IEnumerable<IList<KeyValuePair<DateTime, decimal>>> GetRevenueGroupedBySalesChannel(DateTime from, DateTime to);
        IEnumerable<KeyValuePair<string, decimal>> GetRevenueStructureGroupedBySalesChannel(DateTime from, DateTime to);
    }
    public class OrderAnalyticsService : IOrderAnalyticsService
    {
        private readonly ISession _session;

        public OrderAnalyticsService(ISession session)
        {
            _session = session;
        }

        public IList<KeyValuePair<DateTime, decimal>> GetRevenue(DateTime from, DateTime to)
        {
            var results = _session.QueryOver<Order>()
                                 .Where(item => item.CreatedOn >= from && item.CreatedOn <= to).Cacheable().List()
                                 .GroupBy(c => c.CreatedOn.Date).Select(k => new { Date = k.Key, Sum = k.Sum(t => t.Total) })
                                 .ToDictionary(t => t.Date, t => t.Sum).ToList();

            return results;
        }

        public IEnumerable<IList<KeyValuePair<DateTime, decimal>>> GetRevenueGroupedBySalesChannel(DateTime from, DateTime to)
        {
            var results = new List<IList<KeyValuePair<DateTime, decimal>>>();
            var baseData = _session.QueryOver<Order>().Where(item => item.CreatedOn >= from.Date && item.CreatedOn <= to.Date).Cacheable().List().GroupBy(x=>x.SalesChannel);
            if (baseData.ToList().Any(x => x.Key == SalesChannel.MrCMS))
                results.Add(baseData.ToList().SingleOrDefault(x => x.Key == SalesChannel.MrCMS).GroupBy(c => c.CreatedOn.Date)
                    .Select(k => new {Date = k.Key, Sum = k.Sum(t => t.Total)}).ToDictionary(t => t.Date.Date, t => t.Sum).ToList());
            if (baseData.ToList().Any(x => x.Key == SalesChannel.Amazon))
                results.Add(baseData.ToList().SingleOrDefault(x => x.Key == SalesChannel.Amazon).GroupBy(c => c.CreatedOn.Date)
                    .Select(k => new { Date = k.Key, Sum = k.Sum(t => t.Total) }).ToDictionary(t => t.Date.Date, t => t.Sum).ToList());
            if (baseData.ToList().Any(x => x.Key == SalesChannel.EBay))
                results.Add(baseData.ToList().SingleOrDefault(x => x.Key == SalesChannel.EBay).GroupBy(c => c.CreatedOn.Date)
                    .Select(k => new { Date = k.Key, Sum = k.Sum(t => t.Total) }).ToDictionary(t => t.Date.Date, t => t.Sum).ToList());
            return results;
        }

        public IEnumerable<KeyValuePair<string, decimal>> GetRevenueStructureGroupedBySalesChannel(DateTime from, DateTime to)
        {
            var baseData =
                _session.QueryOver<Order>()
                        .Where(item => item.CreatedOn >= from.Date && item.CreatedOn <= to.Date)
                        .Cacheable()
                        .List();
            var total = baseData.Sum(x => x.Total);
            return baseData.GroupBy(x=>x.SalesChannel).Select(k => new {k.Key, Sum = (k.Sum(t => t.Total)*100/total) }).ToDictionary(t => t.Key.ToString(), t => t.Sum).ToList();
        }

        public IList<KeyValuePair<DateTime, int>> GetNumberOfOrders(DateTime from, DateTime to)
        {
            var results = _session.QueryOver<Order>()
                                 .Where(item => item.CreatedOn >= from && item.CreatedOn <= to).Cacheable().List()
                                 .GroupBy(c => c.CreatedOn.Date).Select(k => new { Date = k.Key, Count = k.Count() })
                                 .ToDictionary(t => t.Date, t => t.Count).ToList();

            return results;
        }

        public IList<KeyValuePair<DateTime, decimal>> GetProductsSold(DateTime from, DateTime to)
        {
            
            return null;
        }
    }
}