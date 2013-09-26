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

        public AmazonOrderSyncManager(IImportOrdersFromAmazonService importAmazonOrderService,
            IAmazonLogService amazonLogService, IAmazonOrdersApiService amazonOrdersApiService)
        {
            _importAmazonOrderService = importAmazonOrderService;
            _amazonLogService = amazonLogService;
            _amazonOrdersApiService = amazonOrdersApiService;
        }

        public void SyncOrders(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.Clean(model.Task);

            _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, "Checking Amazon Api Service Availability");
            AmazonProgressBarHelper.Update(model.Task, "Api", "Checking Amazon Api Service Availability", 100, 0);

            var serviceStatus = _amazonOrdersApiService.GetServiceStatus(AmazonApiSection.Orders);
            if (serviceStatus == AmazonServiceStatus.GREEN ||
                serviceStatus == AmazonServiceStatus.GREEN_I)
            {
                AmazonProgressBarHelper.Update(model.Task, "Api", AmazonServiceStatus.GREEN.GetDescription(),null, null);
                AmazonProgressBarHelper.Update(model.Task, "Started", "Starting Orders Sync", null, null);

                var orders = _importAmazonOrderService.GetOrdersFromAmazon(model);

                _importAmazonOrderService.ImportOrders(model, orders);

                AmazonProgressBarHelper.Update(model.Task, "Completed", "Completed", 100, 100);
            }
            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Error", AmazonServiceStatus.RED.GetDescription(),100, 0);
            }

        }
    }
}