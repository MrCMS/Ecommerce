using System;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Tasks;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderSyncService
    {
        void Sync();
    }

    public class AmazonOrderSyncService : IAmazonOrderSyncService
    {
        private readonly IAmazonOrderSyncManager _amazonOrderSyncManager;
        private readonly IConfigurationProvider _configurationProvider;

        public AmazonOrderSyncService(IAmazonOrderSyncManager amazonOrderSyncManager, IConfigurationProvider configurationProvider)
        {
            _amazonOrderSyncManager = amazonOrderSyncManager;
            _configurationProvider = configurationProvider;
        }

        public void Sync()
        {
            var amazonSyncSettings = _configurationProvider.GetSiteSettings<AmazonSyncSettings>();
            var lastRun = amazonSyncSettings.LastRun;

            DateTime @from;
            var now = CurrentRequestData.Now;
            var to = now.AddMinutes(-30);
            if (lastRun.HasValue)
            {
                if (lastRun > to)
                    return;
                @from = lastRun.Value;
            }
            else
                @from = now.AddMinutes(-32);
            var updatedOrdersRequest = new GetUpdatedOrdersRequest
            {
                LastUpdatedAfter = @from,
                LastUpdatedBefore = to
            };
            amazonSyncSettings.LastRun = to;
            _configurationProvider.SaveSettings(amazonSyncSettings);
            _amazonOrderSyncManager.GetUpdatedInfoFromAmazon(updatedOrdersRequest);
        }
    }
}