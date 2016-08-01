using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IOrderAnalyticsService
    {
        IList<Sales> GetRevenueWithOrdersGroupedByDate(DateTime from, DateTime to);
        Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> GetRevenueGroupedByDate(DateTime from, DateTime to);
        Dictionary<string, IList<KeyValuePair<string, decimal>>> GetRevenueByPaymentMethod(DateTime from, DateTime to);
        Dictionary<string, IList<KeyValuePair<string, decimal>>> GetRevenueByShippingMethod(DateTime from, DateTime to);
        Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> GetRevenueForTodayGroupedByHour();
        Dictionary<string, IList<KeyValuePair<string, decimal>>> GetOrdersGrouped(DateTime from, DateTime to);
    }
}