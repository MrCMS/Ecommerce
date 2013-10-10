using System;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Tasks
{
    public sealed class ExecuteSyncOfAmazonOrders
    {
        private static ExecuteSyncOfAmazonOrders _instance;
        private static readonly object Padlock = new object();
        private System.Timers.Timer _timer;

        public static ExecuteSyncOfAmazonOrders Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new ExecuteSyncOfAmazonOrders());
                }
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ExecuteTask();
        }
        public void Start(double interval)
        {
            _timer = new System.Timers.Timer(interval)
            {
                Interval = interval * 1000
            };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
      
        private ExecuteSyncOfAmazonOrders()
        {
        }

        private void ExecuteTask()
        {
            _timer.Enabled = false;
            //try
            //{
                var ordersForUpdate = MrCMSApplication.Get<AmazonOrderSyncDataService>().GetAllByOperationType(SyncAmazonOrderOperation.Update);
                foreach (var amazonOrderSyncData in ordersForUpdate)
                {
                    Update(amazonOrderSyncData);
                }
                var ordersForAdd = MrCMSApplication.Get<AmazonOrderSyncDataService>().GetAllByOperationType(SyncAmazonOrderOperation.Add, 100);
                foreach (var amazonOrderSyncData in ordersForAdd)
                {
                    Update(amazonOrderSyncData);
                }
            //}
            //catch (Exception ex)
            //{
            //    CurrentRequestData.ErrorSignal.Raise(ex);
            //}
            _timer.Enabled = true;
        }
        private static AmazonOrder Update(AmazonOrderSyncData data)
        {
            LogStatus(data,SyncAmazonOrderStatus.InProgress);
            var amazonOrder = MrCMSApplication.Get<IUpdateAmazonOrder>().UpdateOrder(data);
            LogStatus(data, SyncAmazonOrderStatus.Synced);
            return amazonOrder;
        }
        private static void LogStatus(AmazonOrderSyncData data,SyncAmazonOrderStatus status)
        {
            data.Status = status;
            MrCMSApplication.Get<AmazonOrderSyncDataService>().Update(data);
        }
    }
}