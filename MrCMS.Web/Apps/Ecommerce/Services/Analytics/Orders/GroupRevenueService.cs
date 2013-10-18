using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IGroupRevenueService
    {
        void AddRevenueGroupedByDateCreated(IEnumerable<IGrouping<SalesChannel, Order>> baseData,
                                           ref List<IList<KeyValuePair<DateTime, decimal>>> results,
                                           SalesChannel salesChannel);

        void AddRevenueGroupedByPaymentType(IEnumerable<IGrouping<SalesChannel, Order>> baseData,
                                            ref List<IList<KeyValuePair<string, decimal>>> results,
                                            SalesChannel salesChannel);

        void AddRevenueGroupedByShippmentType(IEnumerable<IGrouping<SalesChannel, Order>> baseData,
            ref List<IList<KeyValuePair<string, decimal>>> results, SalesChannel salesChannel);

        void AddRevenueGroupedByHour(IEnumerable<IGrouping<SalesChannel, Order>> baseData,
                                            ref List<IList<KeyValuePair<DateTime, decimal>>> results,
                                            SalesChannel salesChannel);
    }

    public class GroupRevenueService : IGroupRevenueService
    {
        public void AddRevenueGroupedByDateCreated(IEnumerable<IGrouping<SalesChannel, Order>> baseData,
            ref List<IList<KeyValuePair<DateTime, decimal>>> results, SalesChannel salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item!=null)
                results.Add(item.GroupBy(c => c.CreatedOn.Date)
                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                    .ToDictionary(t => t.Key.Date, t => t.Sum).ToList());
            else
                results.Add(new List<KeyValuePair<DateTime, decimal>>());
        }

        public void AddRevenueGroupedByPaymentType(IEnumerable<IGrouping<SalesChannel, Order>> baseData, 
            ref List<IList<KeyValuePair<string, decimal>>> results, SalesChannel salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item!=null)
                results.Add(item.Where(x => !String.IsNullOrWhiteSpace(x.PaymentMethod)).GroupBy(c => c.PaymentMethod)
                                    .Select(k => new {k.Key, Sum = k.Sum(t => t.Total)})
                                    .ToDictionary(t => t.Key, t => t.Sum)
                                    .ToList());
            else
                results.Add(new List<KeyValuePair<string, decimal>>());
        }
        public void AddRevenueGroupedByShippmentType(IEnumerable<IGrouping<SalesChannel, Order>> baseData,
            ref List<IList<KeyValuePair<string, decimal>>> results, SalesChannel salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                results.Add(item.Where(x=>x.ShippingMethod!=null).GroupBy(c => c.ShippingMethod.Name)
                                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                                    .ToDictionary(t => t.Key, t => t.Sum)
                                    .ToList());
            else
                results.Add(new List<KeyValuePair<string, decimal>>());
        }

        public void AddRevenueGroupedByHour(IEnumerable<IGrouping<SalesChannel, Order>> baseData,
            ref List<IList<KeyValuePair<DateTime, decimal>>> results, SalesChannel salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                results.Add(item.GroupBy(c => c.CreatedOn)
                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                    .ToDictionary(t => t.Key, t => t.Sum).ToList());
            else
                results.Add(new List<KeyValuePair<DateTime, decimal>>());
        }
    }
}