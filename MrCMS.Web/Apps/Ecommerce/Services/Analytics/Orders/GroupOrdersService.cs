using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public class GroupOrdersService : IGroupOrdersService
    {
        public IList<KeyValuePair<string, decimal>> GetOrdersGroupedByShipmentType(IEnumerable<IGrouping<string, Order>> baseData, string salesChannel)
        {
            var item = baseData.ToList().SingleOrDefault(x => x.Key == salesChannel);
            if (item != null)
                return item.Where(x => !string.IsNullOrWhiteSpace(x.ShippingMethodName)).GroupBy(c => c.ShippingMethodName)
                    .Select(k => new { k.Key, Sum = k.Count() })
                    .ToDictionary(t => t.Key, t => decimal.Parse(t.Sum.ToString()))
                    .ToList();
            return null;
        }
    }
}