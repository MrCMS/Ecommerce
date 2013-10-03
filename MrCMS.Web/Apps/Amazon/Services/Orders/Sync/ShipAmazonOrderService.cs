using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Feeds;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ShipAmazonOrderService : IShipAmazonOrderService
    {
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;
        private readonly IAmazonRequestService _amazonRequestService;

        public ShipAmazonOrderService(IAmazonFeedsApiService amazonFeedsApiService, 
            IAmazonRequestService amazonRequestService)
        {
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonRequestService = amazonRequestService;
        }

        public void MarkAsShipped(AmazonSyncModel syncModel, AmazonOrder amazonOrder)
        {
            var feed = _amazonFeedsApiService.GetOrderFulfillmentFeed(amazonOrder);

            var submissionId = _amazonRequestService.SubmitOrderFulfillmentFeed(syncModel, feed);

            _amazonRequestService.CheckIfOrderFulfillmentFeedWasProcessed(syncModel, amazonOrder, submissionId);
        }

        public void MarkAsShipped(AmazonSyncModel syncModel, List<AmazonOrder> amazonOrders)
        {
            var feed = _amazonFeedsApiService.GetOrderFulfillmentFeed(amazonOrders);

            var submissionId = _amazonRequestService.SubmitOrderFulfillmentFeed(syncModel, feed);

            _amazonRequestService.CheckIfOrderFulfillmentFeedWasProcessed(syncModel, amazonOrders, submissionId);
        }
    }
}