using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class AmazonListingSyncManager : IAmazonListingSyncManager
    {
        private readonly ISyncAmazonListingService _exportAmazonListingService;
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly ICloseAmazonListingService _closeAmazonListingService;
        private readonly IAmazonListingGroupService _amazonListingGroupService;

        public AmazonListingSyncManager(
            ISyncAmazonListingService exportAmazonListingService, 
            IAmazonListingService amazonListingService, 
            IAmazonLogService amazonLogService, 
            ICloseAmazonListingService closeAmazonListingService, 
            IAmazonListingGroupService amazonListingGroupService)
        {
            _exportAmazonListingService = exportAmazonListingService;
            _amazonListingService = amazonListingService;
            _amazonLogService = amazonLogService;
            _closeAmazonListingService = closeAmazonListingService;
            _amazonListingGroupService = amazonListingGroupService;
        }

        public void SyncAmazonListing(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.Clean(model.Task);

            var amazonListing = _amazonListingService.Get(model.Id);
            if (amazonListing != null)
            {
                _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Stage, null, null, AmazonApiSection.Feeds, null, null, amazonListing, null,
                                      "Preparing listing for Amazon");
                AmazonProgressBarHelper.Update(model.Task, "Push", "Preparing listing for Amazon", 100, 0);

                _exportAmazonListingService.SubmitSingleProductFeed(model, amazonListing);

                AmazonProgressBarHelper.Update(model.Task, "Push", "Amazon Listing successfully synced",100, 100);
            }

            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Push", "No listing to sync", null, null);
            }

            AmazonProgressBarHelper.Update(model.Task, "Completed", "Completed", 100, 100);
        }

        public void SyncAmazonListings(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.Clean(model.Task);

            var amazonListingGroup = _amazonListingGroupService.Get(model.Id);
            if (amazonListingGroup != null)
            {
                _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Stage, null,null,AmazonApiSection.Feeds, null, null,null,
                                      amazonListingGroup, "Preparing listings for Amazon");
                AmazonProgressBarHelper.Update(model.Task, "Push", "Preparing listings for Amazon", 100, 0);

                _exportAmazonListingService.SubmitProductFeeds(model,amazonListingGroup);
               
                AmazonProgressBarHelper.Update(model.Task, "Push", "Amazon Listings successfully synced", 100, 100);
            }

            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Push", "No listing to sync", null, null);
            }

            AmazonProgressBarHelper.Update(model.Task, "Completed", "Completed", 100, 100);
        }

        public void CloseAmazonListing(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.Clean(model.Task);

            var amazonListing = _amazonListingService.Get(model.Id);
            if (amazonListing != null)
            {
                _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Stage, null,null,AmazonApiSection.Feeds, null, null,amazonListing,
                                      null, "Preparing request to close Amazon Listing");
                AmazonProgressBarHelper.Update(model.Task, "Push", "Preparing request to close Amazon Listing", 100, 0);

                _closeAmazonListingService.CloseAmazonListing(model, amazonListing);

                AmazonProgressBarHelper.Update(model.Task, "Push", "Amazon Listing successfully closed",
                                               100, 100);
            }

            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Push", "No listing to close", null, null);
            }

            AmazonProgressBarHelper.Update(model.Task, "Completed", "Completed", 100, 100);
        }

        public void CloseAmazonListings(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.Clean(model.Task);

            var amazonListingGroup = _amazonListingGroupService.Get(model.Id);
            if (amazonListingGroup != null)
            {
                _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Stage, null,null,AmazonApiSection.Feeds, null, null,null,
                                      amazonListingGroup, "Preparing requests to close Amazon Listings");
                AmazonProgressBarHelper.Update(model.Task, "Push", "Preparing requests to close Amazon Listings", 100, 0);

                _closeAmazonListingService.CloseAmazonListings(model, amazonListingGroup);

                AmazonProgressBarHelper.Update(model.Task, "Push", "Amazon Listings successfully closed",
                                               100, 100);
            }

            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Push", "No listings to close", null, null);
            }

            AmazonProgressBarHelper.Update(model.Task, "Completed", "Completed", 100, 100);
        }

        public AmazonSyncModel GetAmazonSyncModel(AmazonListing amazonListing)
        {
            return new AmazonSyncModel()
            {
                Id = amazonListing.Id,
                Title = amazonListing.Title,
                Description = amazonListing.ASIN,
                AmazonListingGroup = amazonListing.AmazonListingGroup
            };
        }

        public AmazonSyncModel GetAmazonSyncModel(AmazonListingGroup amazonListingGroup)
        {
            return new AmazonSyncModel()
            {
                Id = amazonListingGroup.Id,
                Title = amazonListingGroup.Name,
                AmazonListingGroup = amazonListingGroup
            };
        }
    }
}