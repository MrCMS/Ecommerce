using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public class GroupRevenueService : IGroupRevenueService
    {
        public IList<KeyValuePair<DateTime, decimal>> GetRevenueGroupedByDateCreated(IEnumerable<IGrouping<string, Order>> baseData,
            string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                return item.GroupBy(c => c.OrderDate.GetValueOrDefault().Date)
                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                    .ToDictionary(t => t.Key.Date, t => t.Sum).ToList();
            return null;
        }

        public IList<KeyValuePair<string, decimal>> GetRevenueGroupedByPaymentType(IEnumerable<IGrouping<string, Order>> baseData, string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                return item.Where(x => !String.IsNullOrWhiteSpace(x.PaymentMethod)).GroupBy(c => c.PaymentMethod)
                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                    .ToDictionary(t => t.Key.BreakUpString(), t => t.Sum)
                    .ToList();
            return null;
        }
        public IList<KeyValuePair<string, decimal>> GetRevenueGroupedByShipmentType(IEnumerable<IGrouping<string, Order>> baseData,
            string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                return item.Where(x => !string.IsNullOrWhiteSpace(x.ShippingMethodName)).GroupBy(c => c.ShippingMethodName)
                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                    .ToDictionary(t => t.Key.BreakUpString(), t => t.Sum)
                    .ToList();
            return null;
        }

        public IList<KeyValuePair<DateTime, decimal>> GetRevenueGroupedByHour(IEnumerable<IGrouping<string, Order>> baseData,
             string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                return item.GroupBy(c => c.CreatedOn)
                    .Select(k => new { k.Key, Sum = k.Sum(t => t.Total) })
                    .ToDictionary(t => t.Key, t => t.Sum).ToList();
            return null;
        }
    }
}