using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public interface IAmazonApiUsageService
    {
        AmazonApiUsage Save(AmazonApiUsage amazonApiUsage);
        AmazonApiUsage GetForToday(AmazonApiSection? apiSection, string apiOperation);
    }
}