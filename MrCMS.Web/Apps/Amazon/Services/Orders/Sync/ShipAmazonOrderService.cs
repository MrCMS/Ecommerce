using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Feeds;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using System.Linq;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ShipAmazonOrderService : IShipAmazonOrderService
    {
        private readonly ISession _session;
        private readonly IAmazonOrderRequestService _amazonOrderRequestService;
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;

        public ShipAmazonOrderService(ISession session, IAmazonOrderRequestService amazonOrderRequestService, IAmazonFeedsApiService amazonFeedsApiService)
        {
            _session = session;
            _amazonOrderRequestService = amazonOrderRequestService;
            _amazonFeedsApiService = amazonFeedsApiService;
        }

        public List<AmazonOrder> MarkOrdersAsShipped()
        {
            var orders =
                _session.QueryOver<AmazonOrder>()
                        .Where(
                            order =>
                            order.Status == AmazonOrderStatus.Unshipped ||
                            order.Status == AmazonOrderStatus.PartiallyShipped)
                        .Fetch(order => order.Order)
                        .Eager.List();

            var shippedOrders = orders.Where(order => order.Order.ShippingStatus == ShippingStatus.Shipped).ToList();

            var feed = _amazonFeedsApiService.GetOrderFulfillmentFeed(shippedOrders);
            var submissionId = _amazonOrderRequestService.SubmitOrderFulfillmentFeed(feed);
            //_amazonOrderRequestService.CheckIfOrderFulfillmentFeedWasProcessed(shippedOrders, submissionId); ToDO: is this needed?
            return shippedOrders;
        }
    }
}