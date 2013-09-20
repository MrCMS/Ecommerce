using System;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public interface IAmazonAnalyticsService
    {
        void TrackNewApiCall(AmazonApiSection? apiSection, string apiOperation);
        int GetNumberOfOrders(DateTime from, DateTime to);
        double GetAverageOrderAmount(DateTime from, DateTime to);
        int GetNumberUnshippedOrders(DateTime from, DateTime to);
        int GetNumberOfActiveListings();
        int GetNumberOfApiCalls(DateTime from, DateTime to);
        double GetNumberOfOrderedProducts(DateTime from, DateTime to);
        double GetNumberOfShippedProducts(DateTime from, DateTime to);
    }
}