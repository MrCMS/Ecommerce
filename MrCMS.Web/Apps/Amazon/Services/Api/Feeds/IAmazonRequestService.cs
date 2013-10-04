using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public interface IAmazonRequestService
    {
        List<string> SubmitMainFeeds(AmazonSyncModel model, List<FileStream> feeds);
        string SubmitCloseRequest(AmazonSyncModel model, FileStream productFeedContent);

        void CheckIfDeleteRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, string submissionId);
        void CheckIfRequestsWhereProcessed(AmazonSyncModel model, AmazonListingGroup item, List<string> submissionIds);
        void CheckIfRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, List<string> submissionIds);

    }

    public interface IAmazonOrderRequestService
    {
        string SubmitOrderFulfillmentFeed(AmazonSyncModel model, FileStream feedContent);
        void CheckIfOrderFulfillmentFeedWasProcessed(AmazonSyncModel model, AmazonOrder amazonOrder,string submissionId);
        void CheckIfOrderFulfillmentFeedWasProcessed(AmazonSyncModel model, List<AmazonOrder> amazonOrders,string submissionId);
    }

    public class AmazonOrderRequestService : IAmazonOrderRequestService
    {
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IOrderService _orderService;

        public AmazonOrderRequestService(IAmazonFeedsApiService amazonFeedsApiService, IAmazonOrderService amazonOrderService, IOrderService orderService)
        {
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonOrderService = amazonOrderService;
            _orderService = orderService;
        }


        public string SubmitOrderFulfillmentFeed(AmazonSyncModel model, FileStream feedContent)
        {
            if (feedContent == null) return null;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing Order Fulfillment Data", 100, 85);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_ORDER_FULFILLMENT_DATA_, feedContent);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Order Fulfillment Data Pushed", 100, 100);
            return feedResponse.FeedSubmissionId;
        }
        public void CheckIfOrderFulfillmentFeedWasProcessed(AmazonSyncModel model, AmazonOrder amazonOrder, string submissionId)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 3)
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
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Order #" + amazonOrder.AmazonOrderId, 100, 75);

                        _amazonOrderService.MarkAsShipped(amazonOrder);

                        AmazonProgressBarHelper.Update(model.Task, "Push", "Local Amazon Order data successfully updated", 100, 85);

                        if (amazonOrder.Order.ShippingStatus == ShippingStatus.Unshipped)
                        {
                            AmazonProgressBarHelper.Update(model.Task, "Push", "Updating status of MrCMS Order #" + amazonOrder.Order.Id, 100, 85);

                            amazonOrder.Order.PaymentMethod = AmazonPaymentMethod.Other.GetDescription();
                            amazonOrder.Order.PaymentStatus = PaymentStatus.Paid;
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

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will wait additional 2 min. and try again...", 100,
                                                   75);
                    Thread.Sleep(120000);
                }
            }
        }
        public void CheckIfOrderFulfillmentFeedWasProcessed(AmazonSyncModel model, List<AmazonOrder> amazonOrders, string submissionId)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 3)
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
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);

                        foreach (var amazonOrder in amazonOrders)
                        {
                            AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Order #" + amazonOrder.AmazonOrderId, 100, 75);
                            _amazonOrderService.MarkAsShipped(amazonOrder);
                        }

                        AmazonProgressBarHelper.Update(model.Task, "Push", "Local Amazon Order data successfully updated", 100, 85);

                        foreach (var amazonOrder in amazonOrders)
                        {
                            if (amazonOrder.Order.ShippingStatus != ShippingStatus.Unshipped) continue;

                            AmazonProgressBarHelper.Update(model.Task, "Push", "Updating status of MrCMS Order #" + amazonOrder.Order.Id, 100, 85);

                            amazonOrder.Order.PaymentMethod = AmazonPaymentMethod.Other.GetDescription();
                            amazonOrder.Order.PaymentStatus = PaymentStatus.Paid;
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

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will wait additional 2 min. and try again...", 100,
                                                   75);
                    Thread.Sleep(120000);
                }
            }
        }
    }
}