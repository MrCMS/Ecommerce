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

        private void ProcessOrder(Order order, ref AmazonOrder amazonOrder, bool newOrder)
        {
            foreach (var update in _amazonUpdates.OrderBy(updates => updates.Order))
                update.Update(amazonOrder, order);

            _amazonOrderService.SaveOrUpdate(amazonOrder);

            if (newOrder)
                foreach (var orderPlaced in _onAmazonOrderPlaceds)
                    orderPlaced.OnAmazonOrderPlaced(amazonOrder);
        }

        public AmazonOrder UpdateOrder(AmazonOrderSyncData amazonOrderSyncData)
        {
            var byAmazonOrderId = _amazonOrderService.GetByAmazonOrderId(amazonOrderSyncData.OrderId);
            if (amazonOrderSyncData.Operation == SyncAmazonOrderOperation.Add && byAmazonOrderId != null)
            {
                    amazonOrderSyncData.Status = SyncAmazonOrderStatus.Synced;
                    amazonOrderSyncData.AmazonOrder = byAmazonOrderId;
                    _amazonOrderSyncDataService.Update(amazonOrderSyncData);
                    _amazonOrderSyncDataService.Delete(amazonOrderSyncData);
                    return byAmazonOrderId;
            }
            if (amazonOrderSyncData.AmazonOrder == null && byAmazonOrderId!=null)
            {
                amazonOrderSyncData.AmazonOrder = byAmazonOrderId;
                _amazonOrderSyncDataService.Update(amazonOrderSyncData);
            }

            var amazonOrder = amazonOrderSyncData.AmazonOrder ?? new AmazonOrder();
            var order = AmazonAppHelper.DeserializeFromJson<Order>(amazonOrderSyncData.Data);

            ProcessOrder(order, ref amazonOrder, amazonOrderSyncData.Operation == SyncAmazonOrderOperation.Add);

            amazonOrderSyncData.Status = SyncAmazonOrderStatus.Synced;
            amazonOrderSyncData.AmazonOrder = amazonOrder;
            _amazonOrderSyncDataService.Update(amazonOrderSyncData);
            _amazonOrderSyncDataService.Delete(amazonOrderSyncData);

            return amazonOrder;

        }
    }
}