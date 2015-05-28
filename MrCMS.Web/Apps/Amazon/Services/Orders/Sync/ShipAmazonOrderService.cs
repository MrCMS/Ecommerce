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
        private readonly IAmazonOrderService _amazonOrderService;

        public ShipAmazonOrderService(ISession session, IAmazonOrderRequestService amazonOrderRequestService,
            IAmazonFeedsApiService amazonFeedsApiService, IAmazonOrderService amazonOrderService)
        {
            _session = session;
            _amazonOrderRequestService = amazonOrderRequestService;
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonOrderService = amazonOrderService;
        }

        public List<AmazonOrder> MarkOrdersAsShipped()
        {
            var orders =
                _session.QueryOver<AmazonOrder>()
                    .Where(
                        order =>
                            (order.Status == AmazonOrderStatus.Unshipped ||
                             order.Status == AmazonOrderStatus.PartiallyShipped)
                            && order.Order != null)
                    .Fetch(order => order.Order)
                    .Eager.List();

            var shippedOrders = orders.Where(order => order.Order.ShippingStatus == ShippingStatus.Shipped).ToList();

            var feed = _amazonFeedsApiService.GetOrderFulfillmentFeed(shippedOrders);
            _amazonOrderRequestService.SubmitOrderFulfillmentFeed(feed);
            foreach (var amazonOrder in shippedOrders)
                _amazonOrderService.MarkAsShipped(amazonOrder);

            return shippedOrders;
        }
    }
}