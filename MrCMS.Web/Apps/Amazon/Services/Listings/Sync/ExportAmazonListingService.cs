using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class ExportAmazonListingService : IExportAmazonListingService
    {
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonApiService _amazonApiService;

        public ExportAmazonListingService(IAmazonListingService amazonListingService,
                                          IAmazonApiService amazonApiService)
        {
            _amazonListingService = amazonListingService;
            _amazonApiService = amazonApiService;
        }

        public void SubmitProductFeeds(AmazonSyncModel model, AmazonListingGroup item)
        {
            var productFeedsContent = _amazonApiService.GetProductFeedsContent(item);
            var productPriceFeedsContent = _amazonApiService.GetProductPriceFeedsContent(item);
            var productInventorysFeedContent = _amazonApiService.GetProductInventoryFeedsContent(item);

            var submissionIds = SubmitMainFeeds(model, productFeedsContent, productPriceFeedsContent, productInventorysFeedContent);

            SubmitImageFeeds(model, item, submissionIds);
        }
        private void SubmitImageFeeds(AmazonSyncModel model, AmazonListingGroup item, List<string> submissionIds)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            var feedContent = _amazonApiService.GetProductImageFeedsContent(item);
            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonApiService.GetFeedSubmissionList(submissionIds.First()).FeedProcessingStatus ==
                        "_DONE_")
                    {
                        foreach (var amazonListing in item.Items)
                        {
                            UpdateAmazonListing(amazonListing,null);
                        }

                        SubmitProductImageFeed(model, feedContent, ref submissionIds);

                        uploadSuccess = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Nothing yet, we will wait 2 min. more and try again...", 100, 75);
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);

                    retryCount++;
                    if (retryCount == 3) break;

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will wait additional 2 min. and try again...", 100,
                                                   75);
                    Thread.Sleep(120000);
                }
            }
        }

        public void SubmitSingleProductFeed(AmazonSyncModel model, AmazonListing item)
        {
            var productFeedContent = _amazonApiService.GetSingleProductFeedContent(item);
            var productPriceFeedContent = _amazonApiService.GetSingleProductPriceFeedContent(item);
            var productInventoryFeedContent = _amazonApiService.GetSingleProductInventoryFeedContent(item);

            var submissionIds = SubmitMainFeeds(model, productFeedContent, productPriceFeedContent, productInventoryFeedContent);

            SubmitImageFeed(model, item, submissionIds);
        }
        private void SubmitImageFeed(AmazonSyncModel model, AmazonListing item, List<string> submissionIds)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            var feedContent = _amazonApiService.GetSingleProductImageFeedContent(item);
            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100,75);
                    var amazonProduct = _amazonApiService.GetMatchingProductForId(item.SellerSKU);
                    if (amazonProduct!=null)
                    {
                        UpdateAmazonListing(item, amazonProduct);

                        if (feedContent != null)
                        {
                            SubmitProductImageFeed(model, feedContent, ref submissionIds);
                        }

                        uploadSuccess = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Nothing yet, we will wait 2 min. more and try again...", 100, 75);
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);

                    retryCount++;
                    if (retryCount == 3) break;

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will wait additional 2 min. and try again...", 100,
                                                   75);
                    Thread.Sleep(120000);
                }
            }
        }

        private void UpdateAmazonListing(AmazonListing item, Product amazonProduct)
        {
            if(amazonProduct==null)
                amazonProduct = _amazonApiService.GetMatchingProductForId(item.SellerSKU);

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
                    CurrentRequestData.ErrorSignal.Raise(ex);

                    retryCount++;
                    if (retryCount == 3) break;

                    Thread.Sleep(120000);
                }
            }
            return submissionIds;
        }

        private void SubmitProductImageFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product image", 100, 75);

            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_IMAGE_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);

            AmazonProgressBarHelper.Update(model.Task, "Push", "Product image pushed", 100, 100);
        }
        private void SubmitProductFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product details", 100, 0);
            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product details pushed", 100, 25);
        }
        private void SubmitProductPriceFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product pricing information", 100, 25);
            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_PRICING_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product pricing information pushed", 100, 50);
        }
        private void SubmitProductInventoryFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product inventory information", 100, 50);
            var feedResponse = _amazonApiService.SubmitFeed(AmazonFeedType._POST_INVENTORY_AVAILABILITY_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product inventory information pushed", 100, 75);
        }
    }
}