using System;
using System.Threading;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Feeds;
using MrCMS.Web.Apps.Amazon.Services.Logs;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class CloseAmazonListingService : ICloseAmazonListingService
    {
        private readonly IAmazonListingRequestService _amazonRequestService;
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonListingService _amazonListingService;

        public CloseAmazonListingService(IAmazonFeedsApiService amazonFeedsApiService,
                                          IAmazonLogService amazonLogService, 
            IAmazonListingRequestService amazonRequestService, 
            IAmazonListingService amazonListingService)
        {
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonLogService = amazonLogService;
            _amazonRequestService = amazonRequestService;
            _amazonListingService = amazonListingService;
        }

        public void CloseAmazonListings(AmazonSyncModel model, AmazonListingGroup amazonListingGroup)
        {
            var feedContent = _amazonFeedsApiService.GetProductsDeleteFeeds(amazonListingGroup);

            var submissionId = _amazonRequestService.SubmitCloseRequest(model, feedContent);

            var isUploaded = false;
            var retryCounter = 0;

            while (!isUploaded)
            {
                retryCounter++;
                retryCounter++;
                if (retryCounter == 3)
                {
                    AmazonProgressBarHelper.Update(model.Task, "Error",
                                                  "Request timed out. Please check logs for potential errors and try again later.", 100,
                                                  100);
                    break;
                }

                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonFeedsApiService.GetFeedSubmissionList(submissionId).FeedProcessingStatus == "_DONE_")
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 90);
                        foreach (var amazonListing in amazonListingGroup.Items)
                        {
                            AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Listing #"+amazonListing.SellerSKU, 100, 90);
                            _amazonListingService.UpdateAmazonListingStatus(amazonListing);
                        }

                        isUploaded = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Nothing yet, we will wait 2 min. more and try again...", 100, 75);
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Error, ex, null,
                                          AmazonApiSection.Feeds, null, null,null,null,"Closing Amazon Listings");

                    AmazonProgressBarHelper.Update(model.Task, "Push", "Amazon Api is busy, we will need to wait additional 2 min. and try again", 100, 75);
                    Thread.Sleep(120000);
                }
            }
        }

        public void CloseAmazonListing(AmazonSyncModel syncModel, AmazonListing amazonListing)
        {
            var feedContent = _amazonFeedsApiService.GetSingleProductDeleteFeed(amazonListing);

            var submissionId = _amazonRequestService.SubmitCloseRequest(syncModel, feedContent);

            _amazonRequestService.CheckIfDeleteRequestWasProcessed(syncModel, amazonListing, submissionId);
        }
    }
}