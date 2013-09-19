using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class SyncAmazonListingService : ISyncAmazonListingsService
    {
        private readonly IExportAmazonListingService _exportAmazonListingService;
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonLogService _amazonLogService;

        public SyncAmazonListingService(
            IExportAmazonListingService exportAmazonListingService, 
            IAmazonListingService amazonListingService, 
            IAmazonLogService amazonLogService)
        {
            _exportAmazonListingService = exportAmazonListingService;
            _amazonListingService = amazonListingService;
            _amazonLogService = amazonLogService;
        }

        public void ExportAmazonListing(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.CleanProgressBars(model.TaskId.Value);

            var amazonListing = _amazonListingService.Get(model.Id);
            if (amazonListing != null)
            {
                _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Stage, AmazonApiSection.Feeds, null, null,
                                      null, "Starting export of listing to Amazon");
                AmazonProgressBarHelper.Update(model.TaskId.Value, "Push", "Creating Amazon Listings", 100, 0);

                _exportAmazonListingService.SubmitProductFeeds(model, amazonListing);

                AmazonProgressBarHelper.Update(model.TaskId.Value, "Push", "Successfully created Amazon Listings",
                                               100, 100);
            }

            else
            {
                AmazonProgressBarHelper.Update(model.TaskId.Value, "Push", "No listing to create", null, null);
            }

            AmazonProgressBarHelper.Update(model.TaskId.Value, "Completed", "Completed", 100, 100);
        }
    }
}