using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IGroupRevenueService
    {
        IList<KeyValuePair<DateTime, decimal>> GetRevenueGroupedByDateCreated(
            IEnumerable<IGrouping<string, Order>> baseData,
            string salesChannel);

        IList<KeyValuePair<string, decimal>> GetRevenueGroupedByPaymentType(IEnumerable<IGrouping<string, Order>> baseData,
            string salesChannel);

        IList<KeyValuePair<string, decimal>> GetRevenueGroupedByShipmentType(IEnumerable<IGrouping<string, Order>> baseData,
            string salesChannel);

        IList<KeyValuePair<DateTime, decimal>> GetRevenueGroupedByHour(IEnumerable<IGrouping<string, Order>> baseData,
            string salesChannel);
    }
}