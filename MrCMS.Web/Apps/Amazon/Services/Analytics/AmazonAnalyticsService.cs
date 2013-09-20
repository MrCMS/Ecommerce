using System;
using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public class AmazonAnalyticsService : IAmazonAnalyticsService
    {
        private readonly ISession _session;
        private readonly IAmazonApiUsageService _amazonApiUsageService;

        public AmazonAnalyticsService(IAmazonApiUsageService amazonApiUsageService, ISession session)
        {
            _amazonApiUsageService = amazonApiUsageService;
            _session = session;
        }

        public void TrackNewApiCall(AmazonApiSection? apiSection, string apiOperation)
        {
            var amazonApiUsage = _amazonApiUsageService.GetForToday(apiSection, apiOperation) ?? new AmazonApiUsage()
                {
                    NoOfCalls = 0,
                    Day = CurrentRequestData.Now.Date,
                    ApiSection = apiSection,
                    ApiOperation = apiOperation,
                    Site = CurrentRequestData.CurrentSite
                };
               
            amazonApiUsage.NoOfCalls += 1;

            _amazonApiUsageService.Save(amazonApiUsage);
        }

        public int GetNumberOfOrders(DateTime from,DateTime to)
        {
            return _session.QueryOver<AmazonOrder>()
                           .Where(item => item.PurchaseDate >= from && item.PurchaseDate <= to).RowCount();
        }

        public double GetAverageOrderAmount(DateTime from, DateTime to)
        {
            return _session.CreateCriteria(typeof(AmazonOrder))
                .Add(Restrictions.Between("PurchaseDate", from, to))
                .SetProjection(Projections.Avg("OrderTotalAmount")).UniqueResult<double>();
        }

        public int GetNumberUnshippedOrders(DateTime from, DateTime to)
        {
            return _session.QueryOver<AmazonOrder>()
                           .Where(item => item.PurchaseDate >= from && item.PurchaseDate <= to &&
                               (item.Status==null || item.Status.Value==AmazonOrderStatus.Unshipped)).RowCount();
        }

        public double GetNumberOfOrderedProducts(DateTime from, DateTime to)
        {
            //AmazonOrder amazonOrderAlias = null;
            //AmazonOrderItem amazonOrderItemAlias = null;
            //var rez= _session.QueryOver<AmazonOrderItem>(() => amazonOrderItemAlias)
            //               .JoinAlias(() => amazonOrderItemAlias.AmazonOrder, () => amazonOrderAlias)
            //               .Where(() => amazonOrderAlias.PurchaseDate >= from && amazonOrderAlias.PurchaseDate <= to)
            //               .Select(Projections.Sum<AmazonOrderItem>(acct => amazonOrderItemAlias.QuantityOrdered))
            //               .SingleOrDefault();
            return 0;
        }

        public double GetNumberOfShippedProducts(DateTime from, DateTime to)
        {
            //AmazonOrder amazonOrderAlias = null;
            //AmazonOrderItem amazonOrderItemAlias = null;
            //return _session.QueryOver<AmazonOrderItem>(() => amazonOrderItemAlias)
            //               .JoinAlias(() => amazonOrderItemAlias.AmazonOrder, () => amazonOrderAlias)
            //               .Where(() => amazonOrderAlias.PurchaseDate >= from && amazonOrderAlias.PurchaseDate <= to)
            //               .Select(Projections.Sum<AmazonOrderItem>(acct => amazonOrderItemAlias.QuantityShipped))
            //               .SingleOrDefault<double>();
            return 0;
        }

        public int GetNumberOfActiveListings()
        {
            return _session.QueryOver<AmazonListing>()
                           .Where(item => item.Status==AmazonListingStatus.Active).RowCount();
        }

        public int GetNumberOfApiCalls(DateTime from, DateTime to)
        {
            return _session.CreateCriteria(typeof(AmazonApiUsage))
                .Add(Restrictions.Between("Day",from,to))
                .SetProjection(Projections.Sum("NoOfCalls")).UniqueResult<int>();
        }
    }
}