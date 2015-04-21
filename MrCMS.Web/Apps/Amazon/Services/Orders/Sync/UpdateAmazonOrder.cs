using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using System.Linq;
using MrCMS.Web.Apps.Amazon.Services.Orders.Events;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IUpdateAmazonOrder
    {
        AmazonOrder UpdateOrder(AmazonOrderSyncData amazonOrderSyncData);
    }
    public class UpdateAmazonOrder : IUpdateAmazonOrder
    {
        private readonly IEnumerable<IPerformAmazonUpdates> _amazonUpdates;
        private readonly IEnumerable<IOnAmazonOrderPlaced> _onAmazonOrderPlaceds;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IAmazonOrderSyncDataService _amazonOrderSyncDataService;

        public UpdateAmazonOrder(IEnumerable<IPerformAmazonUpdates> amazonUpdates,
                                 IEnumerable<IOnAmazonOrderPlaced> onAmazonOrderPlaceds,
                                 IAmazonOrderService amazonOrderService, IAmazonOrderSyncDataService amazonOrderSyncDataService)
        {
            _amazonUpdates = amazonUpdates;
            _onAmazonOrderPlaceds = onAmazonOrderPlaceds;
            _amazonOrderService = amazonOrderService;
            _amazonOrderSyncDataService = amazonOrderSyncDataService;
        }

        private bool ProcessOrder(Order order, ref AmazonOrder amazonOrder, bool newOrder)
        {
            foreach (var update in _amazonUpdates.OrderBy(updates => updates.Order))
            {
                if (!update.Update(amazonOrder, order))
                    return false;
            }

            _amazonOrderService.SaveOrUpdate(amazonOrder);

            if (newOrder)
                foreach (var orderPlaced in _onAmazonOrderPlaceds)
                    orderPlaced.OnAmazonOrderPlaced(amazonOrder);

            return true;
        }

        public AmazonOrder UpdateOrder(AmazonOrderSyncData amazonOrderSyncData)
        {
            var existingAmazonOrder = _amazonOrderService.GetByAmazonOrderId(amazonOrderSyncData.OrderId);
            if (amazonOrderSyncData.Operation == SyncAmazonOrderOperation.Add && existingAmazonOrder != null)
            {
                amazonOrderSyncData.Status = SyncAmazonOrderStatus.Synced;
                amazonOrderSyncData.AmazonOrder = existingAmazonOrder;
                _amazonOrderSyncDataService.Update(amazonOrderSyncData);
                _amazonOrderSyncDataService.Delete(amazonOrderSyncData);
                return existingAmazonOrder;
            }

            if (amazonOrderSyncData.AmazonOrder == null && existingAmazonOrder != null)
            {
                amazonOrderSyncData.AmazonOrder = existingAmazonOrder;
                _amazonOrderSyncDataService.Update(amazonOrderSyncData);
            }

            var amazonOrder = amazonOrderSyncData.AmazonOrder = amazonOrderSyncData.AmazonOrder ?? new AmazonOrder();
            var order = AmazonAppHelper.DeserializeFromJson<Order>(amazonOrderSyncData.Data);

            var success = ProcessOrder(order, ref amazonOrder, amazonOrderSyncData.Operation == SyncAmazonOrderOperation.Add);

            if (success)
            {
                amazonOrderSyncData.Status = SyncAmazonOrderStatus.Synced;
                amazonOrderSyncData.AmazonOrder = amazonOrder;
                _amazonOrderSyncDataService.Update(amazonOrderSyncData);
                _amazonOrderSyncDataService.Delete(amazonOrderSyncData);
            }
            else
            {
                amazonOrderSyncData.Status = SyncAmazonOrderStatus.Pending;
                amazonOrderSyncData.AmazonOrder = amazonOrder;
                _amazonOrderSyncDataService.Update(amazonOrderSyncData);
            }

            return amazonOrder;

        }
    }
}