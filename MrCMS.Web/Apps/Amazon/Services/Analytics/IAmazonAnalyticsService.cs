using System;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public interface IAmazonAnalyticsService
    {
        void TrackNewApiCall(AmazonApiSection? apiSection, string apiOperation);
        AmazonDashboardModel GetAmazonDashboardModel(DateTime? from, DateTime? to);
        AmazonChartModel GetRevenue(DateTime from, DateTime to);
        AmazonChartModel GetProductsSold(DateTime from, DateTime to);
        int GetNumberOfOrders(DateTime from, DateTime to);
        double GetAverageOrderAmount(DateTime from, DateTime to);
        int GetNumberUnshippedOrders(DateTime from, DateTime to);
        decimal GetNumberOfOrderedProducts(DateTime from, DateTime to);
        decimal GetNumberOfShippedProducts(DateTime from, DateTime to);
        int GetNumberOfActiveListings();
        int GetNumberOfApiCalls(DateTime from, DateTime to);
    }
}