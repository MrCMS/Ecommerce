using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public class OrderAnalyticsService : IOrderAnalyticsService
    {
        private readonly IGroupOrdersService _groupOrdersService;
        private readonly IGroupRevenueService _groupRevenueService;
        private readonly IRevenueService _revenueService;

        public OrderAnalyticsService(IGroupRevenueService groupRevenueService, IRevenueService revenueService,
            IGroupOrdersService groupOrdersService)
        {
            _groupRevenueService = groupRevenueService;
            _revenueService = revenueService;
            _groupOrdersService = groupOrdersService;
        }

        public IList<Sales> GetRevenueWithOrdersGroupedByDate(DateTime from, DateTime to)
        {
            var revenueGroupedByDate = new List<Sales>();
            var data = _revenueService.GetBaseDataGroupedByDate(from, to);

            revenueGroupedByDate.AddRange(data.Select(x => new Sales
            {
                Date = x.Key,
                OrdersCount = x.Count(),
                OrderItemsCount = x.Select(o => o.OrderLines.Count).Sum(),
                TotalRevenue = x.Sum(o => o.Total)
            }).OrderByDescending(o => o.Date).ToList());

            return revenueGroupedByDate;
        }

        public Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> GetRevenueGroupedByDate(DateTime from, DateTime to)
        {
            var revenueGroupedByDate = new Dictionary<string, IList<KeyValuePair<DateTime, decimal>>>();
            IEnumerable<IGrouping<string, Order>> baseData = _revenueService.GetBaseDataGroupedBySalesChannel(from, to).ToList();
            foreach (string salesChannel in EcommerceApp.SalesChannels)
            {
                IList<KeyValuePair<DateTime, decimal>> data = _groupRevenueService.GetRevenueGroupedByDateCreated(baseData, salesChannel);
                if (data != null)
                    revenueGroupedByDate[salesChannel] = data;
            }
            return revenueGroupedByDate;
        }

        public Dictionary<string, IList<KeyValuePair<string, decimal>>> GetRevenueByPaymentMethod(DateTime @from, DateTime to)
        {
            var results = new Dictionary<string, IList<KeyValuePair<string, decimal>>>();
            IEnumerable<IGrouping<string, Order>> baseData = _revenueService.GetBaseDataGroupedBySalesChannel(from, to).ToList();
            foreach (string salesChannel in EcommerceApp.SalesChannels)
            {
                IList<KeyValuePair<string, decimal>> list;
                list = _groupRevenueService.GetRevenueGroupedByPaymentType(baseData, salesChannel);
                if (list != null)
                    results[salesChannel] = list;
            }
            return results;
        }

        public Dictionary<string, IList<KeyValuePair<string, decimal>>> GetRevenueByShippingMethod(DateTime @from, DateTime to)
        {
            var results = new Dictionary<string, IList<KeyValuePair<string, decimal>>>();
            IEnumerable<IGrouping<string, Order>> baseData = _revenueService.GetBaseDataGroupedBySalesChannel(from, to).ToList();
            foreach (string salesChannel in EcommerceApp.SalesChannels)
            {
                IList<KeyValuePair<string, decimal>> list;
                list = _groupRevenueService.GetRevenueGroupedByShipmentType(baseData, salesChannel);
                if (list != null)
                    results[salesChannel] = list;
            }
            return results;
        }

        public Dictionary<string, IList<KeyValuePair<string, decimal>>> GetRevenueGrouped(string groupBy, DateTime from,
            DateTime to)
        {
            var results = new Dictionary<string, IList<KeyValuePair<string, decimal>>>();
            IEnumerable<IGrouping<string, Order>> baseData = _revenueService.GetBaseDataGroupedBySalesChannel(from, to).ToList();
            foreach (string salesChannel in EcommerceApp.SalesChannels)
            {
                IList<KeyValuePair<string, decimal>> list;
                switch (groupBy)
                {
                    case "PaymentMethod":
                        list = _groupRevenueService.GetRevenueGroupedByPaymentType(baseData, salesChannel);
                        if (list != null)
                            results[salesChannel] = list;
                        break;
                    case "ShippingMethod":
                        list = _groupRevenueService.GetRevenueGroupedByShipmentType(baseData, salesChannel);
                        if (list != null)
                            results[salesChannel] = list;
                        break;
                }
            }
            return results;
        }

        public Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> GetRevenueForTodayGroupedByHour()
        {
            var results = new Dictionary<string, IList<KeyValuePair<DateTime, decimal>>>();
            IEnumerable<IGrouping<string, Order>> baseData = _revenueService.GetBaseDataGroupedBySalesChannel().ToList();
            foreach (string salesChannel in EcommerceApp.SalesChannels)
            {
                IList<KeyValuePair<DateTime, decimal>> list = _groupRevenueService.GetRevenueGroupedByHour(baseData,
                    salesChannel);
                if (list != null)
                    results[salesChannel] = list;
            }
            return results;
        }

        public Dictionary<string, IList<KeyValuePair<string, decimal>>> GetOrdersGrouped(DateTime from, DateTime to)
        {
            var results = new Dictionary<string, IList<KeyValuePair<string, decimal>>>();
            IEnumerable<IGrouping<string, Order>> baseData = _revenueService.GetBaseDataGroupedBySalesChannel(from, to).ToList();
            foreach (string salesChannel in EcommerceApp.SalesChannels)
            {
                IList<KeyValuePair<string, decimal>> list = _groupOrdersService.GetOrdersGroupedByShipmentType(
                    baseData, salesChannel);
                if (list != null)
                    results[salesChannel] = list;
            }
            return results;
        }
    }
}