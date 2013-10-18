using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IOrderAnalyticsService
    {
        IEnumerable<IList<KeyValuePair<DateTime, decimal>>> GetRevenueGroupedByDate(DateTime from, DateTime to);
        IEnumerable<IList<KeyValuePair<string, decimal>>> GetRevenueGrouped(string groupBy,DateTime from, DateTime to);
        IEnumerable<IList<KeyValuePair<DateTime, decimal>>> GetRevenueForTodayGroupedByHour();
    }

    public class OrderAnalyticsService : IOrderAnalyticsService
    {
        private readonly IGroupRevenueService _groupRevenueService;
        private readonly IRevenueService _revenueService;

        public OrderAnalyticsService(IGroupRevenueService groupRevenueService, IRevenueService revenueService)
        {
            _groupRevenueService = groupRevenueService;
            _revenueService = revenueService;
        }

        public IEnumerable<IList<KeyValuePair<DateTime, decimal>>> GetRevenueGroupedByDate(DateTime from, DateTime to)
        {
            var results = new List<IList<KeyValuePair<DateTime, decimal>>>();
            var baseData = _revenueService.GetBaseDataGroupedBySalesChannel(from, to);
            foreach (var salesChannel in Enum.GetValues(typeof(SalesChannel)).OfType<SalesChannel>())
            {
                _groupRevenueService.AddRevenueGroupedByDateCreated(baseData, ref results, salesChannel);
            }
            return results;
        }

        public IEnumerable<IList<KeyValuePair<string, decimal>>> GetRevenueGrouped(string groupBy,DateTime from, DateTime to)
        {
            var results = new List<IList<KeyValuePair<string, decimal>>>();
            var baseData = _revenueService.GetBaseDataGroupedBySalesChannel(from, to);
            foreach (var salesChannel in Enum.GetValues(typeof(SalesChannel)).OfType<SalesChannel>())
            {
                switch (groupBy)
                {
                    case "PaymentMethod":
                        _groupRevenueService.AddRevenueGroupedByPaymentType(baseData, ref results, salesChannel);
                        break;
                    case "ShippingMethod":
                        _groupRevenueService.AddRevenueGroupedByShippmentType(baseData, ref results, salesChannel);
                        break;
                }
               
            }
            return results;
        }

        public IEnumerable<IList<KeyValuePair<DateTime, decimal>>> GetRevenueForTodayGroupedByHour()
        {
            var results = new List<IList<KeyValuePair<DateTime, decimal>>>();
            var baseData = _revenueService.GetBaseDataGroupedBySalesChannel();
            foreach (var salesChannel in Enum.GetValues(typeof(SalesChannel)).OfType<SalesChannel>())
            {
                _groupRevenueService.AddRevenueGroupedByHour(baseData, ref results, salesChannel);
            }
            return results;
        }
    }
}