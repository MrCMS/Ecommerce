using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class AmazonOrderSyncManager : IAmazonOrderSyncManager
    {
        private readonly IAmazonOrdersApiService _amazonOrdersApiService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IUpdateAmazonOrder _updateAmazonOrder;
        private readonly IShipAmazonOrderService _shipAmazonOrderService;

        public AmazonOrderSyncManager(IAmazonOrdersApiService amazonOrdersApiService,
                                      IAmazonOrderService amazonOrderService,
                                      IUpdateAmazonOrder updateAmazonOrder, IShipAmazonOrderService shipAmazonOrderService)
        {
            _amazonOrdersApiService = amazonOrdersApiService;
            _amazonOrderService = amazonOrderService;
            _updateAmazonOrder = updateAmazonOrder;
            _shipAmazonOrderService = shipAmazonOrderService;
        }


        public GetUpdatedOrdersResult GetUpdatedInfoFromAmazon(GetUpdatedOrdersRequest updatedOrdersRequest)
        {
            if (_amazonOrdersApiService.IsLive(AmazonApiSection.Orders))
            {
                var orders = _amazonOrdersApiService.ListUpdatedOrders(updatedOrdersRequest);
                var ordersUpdated = orders.Select(order => _updateAmazonOrder.UpdateOrder(order))
                                      .Where(amazonOrder => amazonOrder != null)
                                      .ToList();

                _amazonOrderService.SaveOrUpdate(ordersUpdated);
                List<AmazonOrder> ordersShipped = _shipAmazonOrderService.MarkOrdersAsShipped();
                return new GetUpdatedOrdersResult { OrdersUpdated = ordersUpdated, OrdersShipped = ordersShipped };
            }
            return new GetUpdatedOrdersResult { ErrorMessage = "The service is not currently live" };
        }

    }
}