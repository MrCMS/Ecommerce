using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public class AmazonAnalyticsService : IAmazonAnalyticsService
    {
        private readonly IAmazonApiUsageService _amazonApiUsageService;

        public AmazonAnalyticsService(IAmazonApiUsageService amazonApiUsageService)
        {
            _amazonApiUsageService = amazonApiUsageService;
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
    }
}