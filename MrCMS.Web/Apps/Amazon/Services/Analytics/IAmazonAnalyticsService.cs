using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public interface IAmazonAnalyticsService
    {
        void TrackNewApiCall(AmazonApiSection? apiSection, string apiOperation);
    }
}