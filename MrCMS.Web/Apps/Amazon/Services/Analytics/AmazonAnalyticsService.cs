using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public class AmazonAnalyticsService : IAmazonAnalyticsService
    {
        private readonly ISession _session;
        private readonly IAmazonApiLogService _amazonApiUsageService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;

        public AmazonAnalyticsService(IAmazonApiLogService amazonApiUsageService, 
            ISession session, 
            AmazonAppSettings amazonAppSettings,
            AmazonSellerSettings amazonSellerSettings)
        {
            _amazonApiUsageService = amazonApiUsageService;
            _session = session;
            _amazonAppSettings = amazonAppSettings;
            _amazonSellerSettings = amazonSellerSettings;
        }

        public AmazonApiLog TrackNewApiCall(AmazonApiSection? apiSection, string apiOperation)
        {
            var amazonApiUsage = new AmazonApiLog()
                {
                    ApiSection = apiSection,
                    ApiOperation = apiOperation,
                    Site = CurrentRequestData.CurrentSite
                };
               
            return _amazonApiUsageService.Save(amazonApiUsage);
        }

        public AmazonChartModel GetRevenue(DateTime from, DateTime to)
        {
            var orders = _session.QueryOver<AmazonOrder>()
                                 .Where(item => item.PurchaseDate >= from && item.PurchaseDate <= to).Cacheable().List()
                                 .GroupBy(c => c.PurchaseDate.Value).Select(k => new { Date = k.Key, Sum = k.Sum(t => t.OrderTotalAmount) })
                                 .ToDictionary(t => t.Date, t => t.Sum).ToList();

            return SetChartModel(from, to, orders);
        }

        public AmazonChartModel GetProductsSold(DateTime from, DateTime to)
        {
            var orders = _session.QueryOver<AmazonOrder>()
                                 .Where(item => item.PurchaseDate >= from && item.PurchaseDate <= to).Cacheable().List()
                                 .GroupBy(c => c.PurchaseDate.Value).Select(k => new { Date = k.Key, Count = k.Sum(t => t.NumberOfItemsShipped) + k.Sum(t => t.NumberOfItemsUnshipped) })
                                 .ToDictionary(t => t.Date, t => Decimal.Parse(t.Count.ToString())).ToList();

            return SetChartModel(from, to, orders);
        }

        private static AmazonChartModel SetChartModel(DateTime from, DateTime to, IList<KeyValuePair<DateTime, decimal>> items)
        {
            var data = new List<decimal>();
            var labels = new List<string>();
            var ts = to - from;
            var factor = (ts.Days/7)+1;
            var oldDate = DateTime.Parse(from.Date.ToString());
            var currentDate = oldDate;

            for (var i = 0; i < 7; i++)
            {
                oldDate = currentDate;
                currentDate = oldDate.AddDays(ts.Days <= 7 ? 1 : factor);
                labels.Add(oldDate.ToString("dd/MM"));
                data.Add(i == 0
                             ? items.Where(x => x.Key.Date == currentDate.Date).Sum(x => x.Value)
                             : items.Where(x => oldDate.Date <= x.Key && x.Key <= currentDate.Date)
                                    .Sum(x => x.Value));
            }

            return new AmazonChartModel
                {
                    Labels = labels,
                    Data = data
                };
        }

        public AmazonDashboardModel GetAmazonDashboardModel(DateTime? from, DateTime? to)
        {
            var model = new AmazonDashboardModel();
            if (from.HasValue)
                model.FilterFrom = from.Value;
            if (to.HasValue)
                model.FilterUntil = to.Value;
            model.NoOfActiveListings = GetNumberOfActiveListings();
            model.NoOfApiCalls = GetNumberOfApiCalls(model.FilterFrom, model.FilterUntil);
            model.NoOfOrders =GetNumberOfOrders(model.FilterFrom, model.FilterUntil);
            model.NoOfUnshippedOrders =GetNumberUnshippedOrders(model.FilterFrom, model.FilterUntil);
            model.AverageOrderAmount = GetAverageOrderAmount(model.FilterFrom, model.FilterUntil);
            model.NoOfOrderedProducts = GetNumberOfOrderedProducts(model.FilterFrom, model.FilterUntil);
            model.NoOfShippedProducts = GetNumberOfShippedProducts(model.FilterFrom, model.FilterUntil);
            model.AppSettingsStatus = AmazonAppHelper.CheckAppSettingsStatus(_amazonAppSettings);
            model.SellerSettingsStatus = AmazonAppHelper.CheckSellerSettingsStatus(_amazonSellerSettings);
            return model;
        }

        public int GetNumberOfOrders(DateTime from, DateTime to)
        {
            return _session.QueryOver<AmazonOrder>()
                           .Where(item => item.PurchaseDate >= from && item.PurchaseDate <= to).Cacheable().RowCount();
        }

        public double GetAverageOrderAmount(DateTime from, DateTime to)
        {
            return _session.CreateCriteria(typeof(AmazonOrder))
                .Add(Restrictions.Between("PurchaseDate", from, to))
                .SetProjection(Projections.Avg("OrderTotalAmount")).SetCacheable(true).UniqueResult<double>();
        }

        public int GetNumberUnshippedOrders(DateTime from, DateTime to)
        {
            return _session.QueryOver<AmazonOrder>().Where(item => item.PurchaseDate >= from && item.PurchaseDate <= to &&
                               item.Status==AmazonOrderStatus.Unshipped).Cacheable().RowCount();
        }

        public decimal GetNumberOfOrderedProducts(DateTime from, DateTime to)
        {
            AmazonOrder amazonOrderAlias = null;
            AmazonOrderItem amazonOrderItemAlias = null;
            return _session.QueryOver(() => amazonOrderItemAlias)
                              .JoinAlias(() => amazonOrderItemAlias.AmazonOrder, () => amazonOrderAlias)
                              .Where(() => amazonOrderAlias.PurchaseDate >= from && amazonOrderAlias.PurchaseDate <= to)
                              .Cacheable().List().Sum(x => x.QuantityOrdered);
        }

        public decimal GetNumberOfShippedProducts(DateTime from, DateTime to)
        {
            AmazonOrder amazonOrderAlias = null;
            AmazonOrderItem amazonOrderItemAlias = null;
            return _session.QueryOver(() => amazonOrderItemAlias)
                              .JoinAlias(() => amazonOrderItemAlias.AmazonOrder, () => amazonOrderAlias)
                              .Where(() => amazonOrderAlias.PurchaseDate >= from && amazonOrderAlias.PurchaseDate <= to)
                              .Cacheable().List().Sum(x => x.QuantityShipped);
        }

        public int GetNumberOfActiveListings()
        {
            return _session.QueryOver<AmazonListing>()
                           .Where(item => item.Status==AmazonListingStatus.Active).Cacheable().RowCount();
        }

        public int GetNumberOfApiCalls(DateTime from, DateTime to)
        {
            return _session.CreateCriteria(typeof(AmazonApiLog))
                .Add(Restrictions.Between("CreatedOn",from,to))
                .SetProjection(Projections.Count("Id")).SetCacheable(true).UniqueResult<int>();
        }
    }
}