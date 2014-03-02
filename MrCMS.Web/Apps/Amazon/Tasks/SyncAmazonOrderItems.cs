using System;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Tasks
{
    public class SyncAmazonOrdersItems : SchedulableTask
    {
        private readonly IAmazonOrderSyncDataService _amazonOrderSyncDataService;
        private readonly IUpdateAmazonOrder _updateAmazonOrder;

        public SyncAmazonOrdersItems(IAmazonOrderSyncDataService amazonOrderSyncDataService, IUpdateAmazonOrder updateAmazonOrder)
        {
            _amazonOrderSyncDataService = amazonOrderSyncDataService;
            _updateAmazonOrder = updateAmazonOrder;
        }

        public override int Priority
        {
            get { return 2; }
        }

        protected override void OnExecute()
        {
            try
            {
                var ordersForAdd = _amazonOrderSyncDataService.GetAllByOperationType(SyncAmazonOrderOperation.Add, 10);
                foreach (var amazonOrderSyncData in ordersForAdd)
                    Update(amazonOrderSyncData);

                var ordersForUpdate = _amazonOrderSyncDataService.GetAllByOperationType(SyncAmazonOrderOperation.Update);
                foreach (var amazonOrderSyncData in ordersForUpdate)
                    Update(amazonOrderSyncData);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
        }
        private void Update(AmazonOrderSyncData data)
        {
            if (data.Status != SyncAmazonOrderStatus.Pending)
                return;
            LogStatus(data, SyncAmazonOrderStatus.InProgress);
            _updateAmazonOrder.UpdateOrder(data);
        }
        private void LogStatus(AmazonOrderSyncData data, SyncAmazonOrderStatus status)
        {
            data.Status = status;
            _amazonOrderSyncDataService.Update(data);
        }
    }
}