using System;
using System.IO;
using System.Threading;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Web.Apps.Amazon.Services.Logs;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class CloseAmazonListingService : ICloseAmazonListingService
    {
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonApiService _amazonApiService;
        private readonly IAmazonLogService _amazonLogService;

        public CloseAmazonListingService(IAmazonListingService amazonListingService,
                                          IAmazonApiService amazonApiService,
                                          IAmazonLogService amazonLogService)
        {
            _amazonListingService = amazonListingService;
            _amazonApiService = amazonApiService;
            _amazonLogService = amazonLogService;
        }

        public void CloseAmazonListings(AmazonSyncModel model, AmazonListingGroup item)
        {
            var feedContent = _amazonApiService.GetDeleteProductFeedsContent(item);

            var submissionId = SubmitCloseRequest(model, feedContent);

            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonApiService.GetFeedSubmissionList(submissionId).FeedProcessingStatus == "_DONE_")
                    {
                        foreach (var amazonListing in item.Items)
                        {
                            UpdateAmazonListing(amazonListing);
                        }

                        uploadSuccess = true;
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
                                          AmazonApiSection.Feeds, null, null,"Closing Amazon Listings");
                    retryCount++;
                    if (retryCount == 3) break;

                    AmazonProgressBarHelper.Update(model.Task, "Push", "Amazon Api is busy, we will need to wait additional 2 min. and try again", 100, 75);
                    Thread.Sleep(120000);
                }
            }
        }

        public void CloseAmazonListing(AmazonSyncModel model, AmazonListing item)
        {
            var productFeedContent = _amazonApiService.GetSingleDeleteProductFeedContent(item);

            var submissionId = SubmitCloseRequest(model, productFeedContent);

            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonApiService.GetFeedSubmissionList(submissionId).FeedProcessingStatus =="_DONE_")
                    {
                        UpdateAmazonListing(item);

                        uploadSuccess = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push","Nothing yet, we will wait 2 min. more and try again...", 100, 75);
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Error, ex, null,
                                          AmazonApiSection.Feeds, null, item);
                    retryCount++;
                    if (retryCount == 3) break;

                    AmazonProgressBarHelper.Update(model.Task, "Push", "Amazon Api is busy, we will need to wait additional 2 min. and try again", 100,75);
                    Thread.Sleep(120000);
                }
            }
        }

        private string SubmitCloseRequest(AmazonSyncModel model, FileStream productFeedContent)
        {
            var submissionId = String.Empty;
            var uploadSuccess = false;
            var retryCount = 0;
            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing request to Amazon", 100, 0);
                    var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_DATA_, productFeedContent);
                    submissionId = feedResponse.FeedSubmissionId;
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Request pushed to Amazon", 100, 75);

                    uploadSuccess = true;
                }
                catch (Exception ex)
                {
                    _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Error, ex, null,
                                          AmazonApiSection.Feeds, null,null,null,"Error during push of product delete request to Amazon");

                    retryCount++;
                    if (retryCount == 3) break;

                    Thread.Sleep(120000);
                }
            }
            return submissionId;
        }

        private void UpdateAmazonListing(AmazonListing item)
        {
            var amazonProduct = _amazonApiService.GetMatchingProductForId(item.SellerSKU);
            if (amazonProduct == null && !String.IsNullOrWhiteSpace(item.ASIN))
            {
                item.Status = AmazonListingStatus.Inactive;
            }
            _amazonListingService.Save(item);
        }
    }
}