using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IGroupRevenueService
    {
        void AddRevenueGroupedByDateCreated(IEnumerable<IGrouping<string, Order>> baseData,
                                           ref List<IList<KeyValuePair<DateTime, decimal>>> results,
                                           string salesChannel);

        void AddRevenueGroupedByPaymentType(IEnumerable<IGrouping<string, Order>> baseData,
                                            ref List<IList<KeyValuePair<string, decimal>>> results,
                                            string salesChannel);

        void AddRevenueGroupedByShippmentType(IEnumerable<IGrouping<string, Order>> baseData,
            ref List<IList<KeyValuePair<string, decimal>>> results, string salesChannel);

        void AddRevenueGroupedByHour(IEnumerable<IGrouping<string, Order>> baseData,
                                            ref List<IList<KeyValuePair<DateTime, decimal>>> results,
                                            string salesChannel);
    }

    public class GroupRevenueService : IGroupRevenueService
    {
        public void AddRevenueGroupedByDateCreated(IEnumerable<IGrouping<string, Order>> baseData,
            ref List<IList<KeyValuePair<DateTime, decimal>>> results, string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                results.Add(item.GroupBy(c => c.CreatedOn.Date)
                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                    .ToDictionary(t => t.Key.Date, t => t.Sum).ToList());
            else
                results.Add(new List<KeyValuePair<DateTime, decimal>>());
        }

        public void AddRevenueGroupedByPaymentType(IEnumerable<IGrouping<string, Order>> baseData,
            ref List<IList<KeyValuePair<string, decimal>>> results, string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                results.Add(item.Where(x => !String.IsNullOrWhiteSpace(x.PaymentMethod)).GroupBy(c => c.PaymentMethod)
                                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                                    .ToDictionary(t => t.Key, t => t.Sum)
                                    .ToList());
            else
                results.Add(new List<KeyValuePair<string, decimal>>());
        }
        public void AddRevenueGroupedByShippmentType(IEnumerable<IGrouping<string, Order>> baseData,
            ref List<IList<KeyValuePair<string, decimal>>> results, string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                results.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.ShippingMethodName)).GroupBy(c => c.ShippingMethodName)
                                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                                    .ToDictionary(t => t.Key, t => t.Sum)
                                    .ToList());
            else
                results.Add(new List<KeyValuePair<string, decimal>>());
        }

        public void AddRevenueGroupedByHour(IEnumerable<IGrouping<string, Order>> baseData,
            ref List<IList<KeyValuePair<DateTime, decimal>>> results, string salesChannel)
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