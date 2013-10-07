using System;
using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Logs
{
    public interface IAmazonApiLogService
    {
        AmazonApiLog Save(AmazonApiLog amazonApiUsage);
        AmazonApiLog GetByTimeRange(AmazonApiSection? apiSection, string apiOperation, DateTime from, DateTime to);
    }
}