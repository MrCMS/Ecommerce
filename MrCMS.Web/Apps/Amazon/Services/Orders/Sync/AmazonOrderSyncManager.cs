using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Orders;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class AmazonOrderSyncManager : IAmazonOrderSyncManager
    {
        private readonly IImportOrdersFromAmazonService _importAmazonOrderService;
        private readonly IAmazonOrdersApiService _amazonOrdersApiService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IShipAmazonOrderService _shipAmazonOrderService;

        public AmazonOrderSyncManager(IImportOrdersFromAmazonService importAmazonOrderService,
            IAmazonLogService amazonLogService, IAmazonOrdersApiService amazonOrdersApiService, 
            IAmazonOrderService amazonOrderService, IShipAmazonOrderService cancelAmazonOrderService)
        {
            _importAmazonOrderService = importAmazonOrderService;
            _amazonLogService = amazonLogService;
            _amazonOrdersApiService = amazonOrdersApiService;
            _amazonOrderService = amazonOrderService;
            _shipAmazonOrderService = cancelAmazonOrderService;
        }

        public void SyncOrders(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.Clean(model.Task);

            _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage,null,null, AmazonApiSection.Orders,null,null,null,null, "Checking Amazon Api Service Availability");
            AmazonProgressBarHelper.Update(model.Task, "Api", "Checking Amazon Api Service Availability", 100, 0);

            var serviceStatus = _amazonOrdersApiService.GetServiceStatus(AmazonApiSection.Orders);
            if (serviceStatus == AmazonServiceStatus.GREEN || serviceStatus == AmazonServiceStatus.GREEN_I)
            {
                AmazonProgressBarHelper.Update(model.Task, "Api", AmazonServiceStatus.GREEN.GetDescription(),null, null);
                AmazonProgressBarHelper.Update(model.Task, "Started", "Starting Orders Sync", null, null);

                var outOfSyncAmazonOrders = new List<AmazonOrder>();

                var orders = _importAmazonOrderService.GetOrdersFromAmazon(model, ref outOfSyncAmazonOrders);

                _importAmazonOrderService.ImportOrders(model, orders);

                if(outOfSyncAmazonOrders.Any())
                    _shipAmazonOrderService.MarkAsShipped(model, outOfSyncAmazonOrders);

                AmazonProgressBarHelper.Update(model.Task, "Completed", "Completed", 100, 100);
            }
            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Error", AmazonServiceStatus.RED.GetDescription(),100, 0);
            }

        }

        public void ShipOrder(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.Clean(model.Task);

            var order = _amazonOrderService.Get(model.Id);

            if (order != null)
            {
                    AmazonProgressBarHelper.Update(model.Task, "Started", "Preparing request to mark Amazon Order as Shipped",null, null);

                    _shipAmazonOrderService.MarkAsShipped(model, order);

                    AmazonProgressBarHelper.Update(model.Task, "Completed", "Completed", 100, 100);
            }
            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Error", "No order specified", 100, 100);
            }
        }
    }
}