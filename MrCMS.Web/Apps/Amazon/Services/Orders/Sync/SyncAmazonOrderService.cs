using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class SyncAmazonOrderService : ISyncAmazonOrderService
    {
        private readonly IImportAmazonOrderService _importAmazonOrderService;
        private readonly IAmazonApiService _amazonApiService;
        private readonly IAmazonLogService _amazonLogService;

        public SyncAmazonOrderService(IImportAmazonOrderService importAmazonOrderService, 
            IAmazonApiService amazonApiService, IAmazonLogService amazonLogService)
        {
            _importAmazonOrderService = importAmazonOrderService;
            _amazonApiService = amazonApiService;
            _amazonLogService = amazonLogService;
        }

        public void SyncOrders(AmazonSyncModel model)
        {
            AmazonProgressBarHelper.CleanProgressBars(model.Task);

            _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, "Checking Amazon Api Service Availability");
            AmazonProgressBarHelper.Update(model.Task, "Api", "Checking Amazon Api Service Availability", 100, 0);

            var serviceStatus = _amazonApiService.GetServiceStatus(AmazonApiSection.Orders);
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