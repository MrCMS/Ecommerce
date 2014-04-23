using System;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Tasks
{
    public class SyncAmazonOrders : SchedulableTask
    {
        private readonly IAmazonOrderSyncService _amazonOrderSyncService;

        public SyncAmazonOrders(IAmazonOrderSyncService amazonOrderSyncService)
        {
            _amazonOrderSyncService = amazonOrderSyncService;
        }

        public override int Priority
        {
            get { return 1; }
        }

        protected override void OnExecute()
        {
            try
            {
                _amazonOrderSyncService.Sync();
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }

        }
    }
}