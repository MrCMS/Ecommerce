using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IOrderAnalyticsService
    {
        Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> GetRevenueGroupedByDate(DateTime from, DateTime to);

        Dictionary<string, IList<KeyValuePair<string, decimal>>> GetRevenueByPaymentMethod(DateTime from,
            DateTime to);

        Dictionary<string, IList<KeyValuePair<string, decimal>>> GetRevenueByShippingMethod(DateTime from,
            DateTime to);

        Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> GetRevenueForTodayGroupedByHour();
        Dictionary<string, IList<KeyValuePair<string, decimal>>> GetOrdersGrouped(DateTime from, DateTime to);
    }
}