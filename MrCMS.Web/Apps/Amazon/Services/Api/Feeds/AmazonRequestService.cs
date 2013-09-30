using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Products;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public class AmazonRequestService : IAmazonRequestService
    {
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;
        private readonly IAmazonProductsApiService _amazonProductsApiService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IOrderService _orderService;

        public AmazonRequestService(IAmazonListingService amazonListingService,
                                          IAmazonLogService amazonLogService, 
            IAmazonFeedsApiService amazonFeedsApiService, 
            IAmazonProductsApiService amazonProductsApiService,
            IAmazonOrderService amazonOrderService, IOrderService orderService)
        {
            _amazonListingService = amazonListingService;
            _amazonLogService = amazonLogService;
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonProductsApiService = amazonProductsApiService;
            _amazonOrderService = amazonOrderService;
            _orderService = orderService;
        }

        public List<string> SubmitMainFeeds(AmazonSyncModel model, List<FileStream> feeds)
        {
            var submissionIds = new List<string>();
            var uploadSuccess = false;
            var retryCount = 0;
            while (!uploadSuccess)
            {
                try
                {
                    if (feeds.Count >= 1)
                        SubmitProductFeed(model, feeds.First(), ref submissionIds);

                    if (feeds.Count >= 2)
                        SubmitProductPriceFeed(model, feeds[1], ref submissionIds);

                    if (feeds.Count >= 3)
                        SubmitProductInventoryFeed(model, feeds[2], ref submissionIds);

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
        public string SubmitCloseRequest(AmazonSyncModel model, FileStream productFeedContent)
        {
            var submissionId = String.Empty;
            var uploadSuccess = false;
            var retryCount = 0;
            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing request to Amazon", 100, 0);
                    var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_DATA_, productFeedContent);
                    submissionId = feedResponse.FeedSubmissionId;
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Request pushed to Amazon", 100, 75);

                    uploadSuccess = true;
                }
                catch (Exception ex)
                {
                    _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Error, ex, null,
                                          AmazonApiSection.Feeds, null, null, null, "Error during push of product delete request to Amazon");

                    retryCount++;
                    if (retryCount == 3) break;

                    Thread.Sleep(120000);
                }
            }
            return submissionId;
        }

        public void CheckIfDeleteRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, string submissionId)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonFeedsApiService.GetFeedSubmissionList(submissionId).FeedProcessingStatus == "_DONE_")
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Updating local status of Amazon Listing with SKU:" + amazonListing.SellerSKU,
                                                       100, 85);
                        _amazonListingService.UpdateAmazonListingStatus(amazonListing);

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
                    _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Error, ex, null,
                                          AmazonApiSection.Feeds, null, amazonListing);
                    retryCount++;
                    if (retryCount == 3) break;

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will need to wait additional 2 min. and try again",
                                                   100, 75);
                    Thread.Sleep(120000);
                }
            }
        }
        public void CheckIfRequestsWhereProcessed(AmazonSyncModel model, AmazonListingGroup item, List<string> submissionIds)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            var feedContent = _amazonFeedsApiService.GetProductsImageFeeds(item);
            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonFeedsApiService.GetFeedSubmissionList(submissionIds.First()).FeedProcessingStatus ==
                        "_DONE_")
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        foreach (var amazonListing in item.Items)
                        {
                            AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Listing with SKU:" + amazonListing.SellerSKU, 100, 85);
                            _amazonListingService.UpdateAmazonListingStatusAndAsin(amazonListing, null);
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
        public void CheckIfRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, List<string> submissionIds)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            var feedContent = _amazonFeedsApiService.GetSingleProductImageFeed(amazonListing);
            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    var amazonProduct = _amazonProductsApiService.GetMatchingProductForId(amazonListing.SellerSKU);
                    if (amazonProduct != null)
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Listing with SKU:" + amazonListing.SellerSKU, 100, 85);
                        _amazonListingService.UpdateAmazonListingStatusAndAsin(amazonListing, amazonProduct);

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

        private void SubmitProductFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product details", 100, 0);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product details pushed", 100, 25);
        }
        private void SubmitProductPriceFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product pricing information", 100, 25);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_PRICING_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product pricing information pushed", 100, 50);
        }
        private void SubmitProductInventoryFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product inventory information", 100, 50);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_INVENTORY_AVAILABILITY_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product inventory information pushed", 100, 75);
        }
        private void SubmitProductImageFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product image", 100, 85);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_IMAGE_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product image pushed", 100, 100);
        }

        public string SubmitOrderFulfillmentFeed(AmazonSyncModel model, FileStream feedContent)
        {
            if (feedContent == null) return null;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing order request", 100, 85);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_ORDER_FULFILLMENT_DATA_, feedContent);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Order request pushed", 100, 100);
            return feedResponse.FeedSubmissionId;
        }
        public void CheckIfOrderFulfillmentRequestWasProcessed(AmazonSyncModel model, AmazonOrder amazonOrder, string submissionId)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonFeedsApiService.GetFeedSubmissionList(submissionId).FeedProcessingStatus == "_DONE_")
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Order #" + amazonOrder.AmazonOrderId, 100, 75);

                        amazonOrder.Status = AmazonOrderStatus.Shipped;
                        _amazonOrderService.Save(amazonOrder);

                        AmazonProgressBarHelper.Update(model.Task, "Push", "Local Amazon Order data successfully updated", 100, 85);

                        if (amazonOrder.Order.ShippingStatus == ShippingStatus.Unshipped)
                        {
                            AmazonProgressBarHelper.Update(model.Task, "Push", "Updating status of MrCMS Order #" + amazonOrder.Order.Id, 100, 85);
                            _orderService.MarkAsShipped(amazonOrder.Order);
                            AmazonProgressBarHelper.Update(model.Task, "Push", "Status of MrCMS Order #" + amazonOrder.Order.Id + " successfully updated", 100, 100);
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
    }
}