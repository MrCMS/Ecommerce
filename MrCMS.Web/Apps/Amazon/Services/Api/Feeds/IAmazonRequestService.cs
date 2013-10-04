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
        string SubmitOrderFulfillmentFeed(FileStream feedContent);
        void CheckIfOrderFulfillmentFeedWasProcessed( List<AmazonOrder> amazonOrders,string submissionId);
    }

    public class AmazonOrderRequestService : IAmazonOrderRequestService
    {
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;
        private readonly IAmazonOrderService _amazonOrderService;

        public AmazonOrderRequestService(IAmazonFeedsApiService amazonFeedsApiService,
                                         IAmazonOrderService amazonOrderService)
        {
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonOrderService = amazonOrderService;
        }


        public string SubmitOrderFulfillmentFeed(FileStream feedContent)
        {
            if (feedContent == null) return null;

            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_ORDER_FULFILLMENT_DATA_, feedContent);
            return feedResponse.FeedSubmissionId;
        }
        
        public void CheckIfOrderFulfillmentFeedWasProcessed( List<AmazonOrder> amazonOrders, string submissionId)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 3)
                {
                    break;
                }

                try
                {
                    if (_amazonFeedsApiService.GetFeedSubmissionList(submissionId).FeedProcessingStatus == "_DONE_")
                    {
                        foreach (var amazonOrder in amazonOrders)
                            _amazonOrderService.MarkAsShipped(amazonOrder);

                        uploadSuccess = true;
                    }
                    else
                    {
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);
                    Thread.Sleep(120000);
                }
            }
        }
    }
}