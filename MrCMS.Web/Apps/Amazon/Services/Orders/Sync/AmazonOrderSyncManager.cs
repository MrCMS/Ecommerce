using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Web.Apps.Amazon.Services.Api.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class AmazonOrderSyncManager : IAmazonOrderSyncManager
    {
        private readonly IAmazonOrdersApiService _amazonOrdersApiService;
        private readonly IUpdateAmazonOrder _updateAmazonOrder;
        private readonly IScheduleAmazonOrderSync _scheduleAmazonOrderSync;
        private readonly IShipAmazonOrderService _shipAmazonOrderService;
        private readonly IAmazonApiService _amazonApiService;

        public AmazonOrderSyncManager(IAmazonOrdersApiService amazonOrdersApiService,
            IShipAmazonOrderService shipAmazonOrderService, IAmazonApiService amazonApiService, 
            IUpdateAmazonOrder updateAmazonOrder, IScheduleAmazonOrderSync scheduleAmazonOrderSync)
        {
            _amazonOrdersApiService = amazonOrdersApiService;
            _shipAmazonOrderService = shipAmazonOrderService;
            _amazonApiService = amazonApiService;
            _updateAmazonOrder = updateAmazonOrder;
            _scheduleAmazonOrderSync = scheduleAmazonOrderSync;
        }


        public GetUpdatedOrdersResult GetUpdatedInfoFromAmazon(GetUpdatedOrdersRequest updatedOrdersRequest)
        {
            if (_amazonApiService.IsLive(AmazonApiSection.Orders))
            {
                var orders = _amazonOrdersApiService.ListUpdatedOrders(updatedOrdersRequest);
                orders.Select(order => _scheduleAmazonOrderSync.ScheduleSync(order))
                                      .Where(amazonOrder => amazonOrder != null)
                                      .ToList();
                var ordersShipped = _shipAmazonOrderService.MarkOrdersAsShipped();
                return new GetUpdatedOrdersResult { OrdersShipped = ordersShipped };
            }
            return new GetUpdatedOrdersResult { ErrorMessage = "The service is not currently live" };
        }

        public GetUpdatedOrdersResult GetUpdatedInfoFromAmazonAdHoc(IEnumerable<string> amazonOrderIds)
        {
            if (_amazonApiService.IsLive(AmazonApiSection.Orders))
            {
                var orders = _amazonOrdersApiService.ListSpecificOrders(amazonOrderIds);
                if (orders.Any())
                {
                    var ordersUpdated = orders.Select(order => _updateAmazonOrder.UpdateOrder(order))
                                             .Where(amazonOrder => amazonOrder != null)
                                             .ToList();
                    return new GetUpdatedOrdersResult {OrdersUpdated = ordersUpdated};
                }
                return new GetUpdatedOrdersResult { ErrorMessage = "We didn't found any Amazon Orders with provided Ids" };
            }
            return new GetUpdatedOrdersResult { ErrorMessage = "The service is not currently live" };
        }
    }
}