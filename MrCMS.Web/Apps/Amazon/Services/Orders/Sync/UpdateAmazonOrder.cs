using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using System.Linq;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class UpdateAmazonOrder : IUpdateAmazonOrder
    {
        private readonly IEnumerable<IPerformAmazonUpdates> _amazonUpdates;
        private readonly IImportAmazonOrderService _importAmazonOrderService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonOrderService _amazonOrderService;

        public UpdateAmazonOrder(IEnumerable<IPerformAmazonUpdates> amazonUpdates,
                                 IImportAmazonOrderService importAmazonOrderService, IAmazonLogService amazonLogService,
                                 IAmazonOrderService amazonOrderService)
        {
            _amazonUpdates = amazonUpdates;
            _importAmazonOrderService = importAmazonOrderService;
            _amazonLogService = amazonLogService;
            _amazonOrderService = amazonOrderService;
        }

        public AmazonOrder UpdateOrder(Order order)
        {
            switch (order.OrderStatus)
            {
                case OrderStatusEnum.Unshipped:
                case OrderStatusEnum.PartiallyShipped:
                case OrderStatusEnum.Shipped:
                case OrderStatusEnum.Canceled:
                    break;
                default:
                    return null;
            }
            if (!_importAmazonOrderService.IsCurrencyValid(order))
            {
                _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, null, null, AmazonApiSection.Orders,
                                      null, null, null, null,
                                      string.Format("Amazon Order #{0} uses different currency than current MrCMS Site.", order.AmazonOrderId));
                return null;
            }
            var amazonOrder = _amazonOrderService.GetByAmazonOrderId(order.AmazonOrderId);
            if (amazonOrder == null && order.OrderStatus == OrderStatusEnum.Canceled)
                return null;
            amazonOrder = amazonOrder ?? new AmazonOrder();

            foreach (var update in _amazonUpdates.OrderBy(updates => updates.Order))
                update.Update(amazonOrder, order);

            return amazonOrder;
        }
    }
}