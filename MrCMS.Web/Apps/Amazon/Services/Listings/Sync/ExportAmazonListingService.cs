using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Web.Apps.Amazon.Services.Logs;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class ExportAmazonListingService : IExportAmazonListingService
    {
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonApiService _amazonApiService;
        private readonly IAmazonLogService _amazonLogService;

        public ExportAmazonListingService(IAmazonListingService amazonListingService,
                                          IAmazonApiService amazonApiService,
                                          IAmazonLogService amazonLogService)
        {
            _amazonListingService = amazonListingService;
            _amazonApiService = amazonApiService;
            _amazonLogService = amazonLogService;
        }

        public void SubmitProductFeeds(AmazonSyncModel model, AmazonListing item)
        {
            var productFeedContent = _amazonApiService.GetProductFeedContent(item);
            var productPriceFeedContent = _amazonApiService.GetProductPriceFeedContent(item);
            var productInventoryFeedContent = _amazonApiService.GetProductInventoryFeedContent(item);

            var submissionIds = SubmitMainFeeds(model, productFeedContent, productPriceFeedContent, productInventoryFeedContent);

            SubmitImageFeeds(model, item, submissionIds);
        }

        private void SubmitImageFeeds(AmazonSyncModel model, AmazonListing item, List<string> submissionIds)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            var feedContent = _amazonApiService.GetProductImageFeedContent(item);
            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if product data was processed...", 100,
                                                   75);
                    if (_amazonApiService.GetFeedSubmissionList(submissionIds.First()).FeedProcessingStatus ==
                        "_DONE_")
                    {
                        UpdateAmazonListing(item, ref submissionIds);

                        SubmitProductImageFeed(model, feedContent, ref submissionIds);

                        uploadSuccess = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Nothing yet, we will wait 2min and try again...", 100, 75);
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount == 3) break;

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will need to wait additional 2min and try again", 100,
                                                   75);
                    Thread.Sleep(120000);
                }
            }
        }

        private void UpdateAmazonListing(AmazonListing item, ref List<string> submissionIds)
        {
            item.AmazonSubmissionId = submissionIds.First();
            var amazonProduct = _amazonApiService.GetMatchingProductForId(item.SellerSKU);
            if (amazonProduct != null && amazonProduct.Identifiers.MarketplaceASIN != null)
            {
                if (String.IsNullOrWhiteSpace(item.ASIN))
                {
                    item.Status = AmazonListingStatus.Active;
                    item.ASIN = amazonProduct.Identifiers.MarketplaceASIN.ASIN;
                }
            }
            else
            {
                item.Status = String.IsNullOrWhiteSpace(item.ASIN)
                                  ? AmazonListingStatus.NotOnAmazon
                                  : AmazonListingStatus.Inactive;
            }
            _amazonListingService.Save(item);
        }

        private List<string> SubmitMainFeeds(AmazonSyncModel model, FileStream productFeedContent, FileStream productPriceFeedContent,
                                     FileStream productInventoryFeedContent)
        {
            var submissionIds = new List<string>();
            var uploadSuccess = false;
            var retryCount = 0;
            while (!uploadSuccess)
            {
                try
                {
                    SubmitProductFeed(model, productFeedContent, ref submissionIds);

                    SubmitProductPriceFeed(model, productPriceFeedContent, ref submissionIds);

                    SubmitProductInventoryFeed(model, productInventoryFeedContent, ref submissionIds);

                    uploadSuccess = true;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount == 3) break;

                    Thread.Sleep(120000);
                }
            }
            return submissionIds;
        }

        private void SubmitProductImageFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product image data", 100, 75);

            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_IMAGE_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);

            AmazonProgressBarHelper.Update(model.Task, "Push", "Product image data pushed", 100, 100);
        }

        private void SubmitProductFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product data", 100, 0);
            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product data pushed", 100, 25);
        }

        private void SubmitProductPriceFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product pricing data", 100, 25);
            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_PRICING_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product pricing data pushed", 100, 50);
        }

        private void SubmitProductInventoryFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product inventory data", 100, 50);
            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_INVENTORY_AVAILABILITY_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product inventory data pushed", 100, 75);
        }
    }
}