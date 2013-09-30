using System;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public interface IAmazonAnalyticsService
    {
        void TrackNewApiCall(AmazonApiSection? apiSection, string apiOperation);
        AmazonDashboardModel GetAmazonDashboardModel(DateTime? from, DateTime? to);
        object GetRevenue(DateTime from, DateTime to);
        object GetProductsSold(DateTime from, DateTime to);
    }
}