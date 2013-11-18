using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Amazon.Controllers
{
    public class AmazonSyncController : MrCMSAppUIController<AmazonApp>
    {
        private readonly IAmazonOrderSyncService _amazonOrderSyncService;
        private readonly IAmazonOrderSyncDataService _amazonOrderSyncDataService;
        private readonly IUpdateAmazonOrder _updateAmazonOrder;

        private readonly object _locker = new object();

        public AmazonSyncController(IAmazonOrderSyncService amazonOrderSyncService, 
            IAmazonOrderSyncDataService amazonOrderSyncDataService, IUpdateAmazonOrder updateAmazonOrder)
        {
            _amazonOrderSyncService = amazonOrderSyncService;
            _amazonOrderSyncDataService = amazonOrderSyncDataService;
            _updateAmazonOrder = updateAmazonOrder;
        }

        public ActionResult Sync()
        {
            _amazonOrderSyncService.Sync();
            return new EmptyResult();
        }

        public ActionResult SyncItems()
        {
            lock (_locker)
            {
                try
                {
                    _amazonOrderSyncDataService.MarkAllAsPendingIfNotSyncedAfterOneHour();
                    var ordersForUpdate =
                        _amazonOrderSyncDataService.GetAllByOperationType(SyncAmazonOrderOperation.Update);
                    foreach (var amazonOrderSyncData in ordersForUpdate)
                    {
                        Update(amazonOrderSyncData);
                    }
                    var ordersForAdd = _amazonOrderSyncDataService.GetAllByOperationType(SyncAmazonOrderOperation.Add,
                                                                                         10);
                    foreach (var amazonOrderSyncData in ordersForAdd)
                    {
                        Update(amazonOrderSyncData);
                    }
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);
                }
                return new EmptyResult();
            }
        }
        private void Update(AmazonOrderSyncData data)
        {
            LogStatus(data, SyncAmazonOrderStatus.InProgress);
            _updateAmazonOrder.UpdateOrder(data);
            LogStatus(data, SyncAmazonOrderStatus.Synced);
        }
        private void LogStatus(AmazonOrderSyncData data, SyncAmazonOrderStatus status)
        {
            data.Status = status;
            _amazonOrderSyncDataService.Update(data);
        }
    }
}