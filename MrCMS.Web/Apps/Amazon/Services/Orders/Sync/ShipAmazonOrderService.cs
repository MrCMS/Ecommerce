using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Feeds;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ShipAmazonOrderService : IShipAmazonOrderService
    {
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;
        private readonly IAmazonOrderRequestService _amazonOrderRequestService;

        public ShipAmazonOrderService(IAmazonFeedsApiService amazonFeedsApiService, 
            IAmazonOrderRequestService amazonOrderRequestService)
        {
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonOrderRequestService = amazonOrderRequestService;
        }

        public void MarkAsShipped(AmazonSyncModel syncModel, AmazonOrder amazonOrder)
        {
            var feed = _amazonFeedsApiService.GetOrderFulfillmentFeed(amazonOrder);

            var submissionId = _amazonOrderRequestService.SubmitOrderFulfillmentFeed(syncModel, feed);

            _amazonOrderRequestService.CheckIfOrderFulfillmentFeedWasProcessed(syncModel, amazonOrder, submissionId);
        }

        public void MarkAsShipped(AmazonSyncModel syncModel, List<AmazonOrder> amazonOrders)
        {
            var feed = _amazonFeedsApiService.GetOrderFulfillmentFeed(amazonOrders);

            var submissionId = _amazonOrderRequestService.SubmitOrderFulfillmentFeed(syncModel, feed);

            _amazonOrderRequestService.CheckIfOrderFulfillmentFeedWasProcessed(syncModel, amazonOrders, submissionId);
        }
    }
}