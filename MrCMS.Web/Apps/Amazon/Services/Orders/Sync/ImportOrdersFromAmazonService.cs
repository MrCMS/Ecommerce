using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Orders;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ImportOrdersFromAmazonService : IImportOrdersFromAmazonService
    {
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IAmazonOrdersApiService _amazonOrdersApiService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IOrderService _orderService;
        private readonly IImportAmazonOrderService _validateAmazonOrderService;

        public ImportOrdersFromAmazonService(
            IAmazonLogService amazonLogService, IOrderService orderService, 
            IImportAmazonOrderService validateAmazonOrderService, 
            IAmazonOrdersApiService amazonOrdersApiService, IAmazonOrderService amazonOrderService)
        {
            _amazonLogService = amazonLogService;
            _orderService = orderService;
            _validateAmazonOrderService = validateAmazonOrderService;
            _amazonOrdersApiService = amazonOrdersApiService;
            _amazonOrderService = amazonOrderService;
        }

        public void ImportOrders(AmazonSyncModel model, ICollection<AmazonOrder> orders)
        {
            if (orders.Any())
            {
                _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders, null,null,null, "Importing Amazon Orders");
                AmazonProgressBarHelper.Update(model.Task, "Import", "Importing Amazon Orders", orders.Count, 0);

                var cnt = 0;
                foreach (var order in orders)
                {
                    _amazonLogService.Add(AmazonLogType.Orders,
                                          order.Id > 0 ? AmazonLogStatus.Update : AmazonLogStatus.Insert,
                                          AmazonApiSection.Orders, order,null,null,"Importing Amazon Order #"+order.AmazonOrderId +" and mapping to MrCMS order");
                    AmazonProgressBarHelper.Update(model.Task, "Import", "Importing Amazon Order #" + order.AmazonOrderId, orders.Count, cnt+1);

                    _orderService.Save(order.Order);
                    _amazonOrderService.Save(order);
                }

                AmazonProgressBarHelper.Update(model.Task, "Import", "Successfully imported Amazon Orders",
                                               orders.Count, 0);
            }
            else
            {
                AmazonProgressBarHelper.Update(model.Task, "Import", "No orders for import",
                                               null, null);
            }
        }

        public List<AmazonOrder> GetOrdersFromAmazon(AmazonSyncModel model)
        {
            var orders = new List<AmazonOrder>();

            _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, null, null, "Getting Orders From Amazon");
            AmazonProgressBarHelper.Update(model.Task, "Import", "Getting Orders From Amazon", 100, 0);

            var rawOrders = model.Id == 0 ? _amazonOrdersApiService.ListOrders(model) : _amazonOrdersApiService.GetOrder(model);

            if (rawOrders != null)
            {
                foreach (var rawOrder in rawOrders.Where(x => x.OrderStatus != OrderStatusEnum.Pending))
                {
                    var amazonOrder = _validateAmazonOrderService.GetAmazonOrder(rawOrder);

                    if (amazonOrder != null)
                    {
                        _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders, null,
                                              "Getting Items From Amazon for Order #" + rawOrder.AmazonOrderId);

                        var rawOrderItems = _amazonOrdersApiService.ListOrderItems(rawOrder.AmazonOrderId);

                        orders.Add(_validateAmazonOrderService.SetAmazonOrderItems(rawOrder, rawOrderItems, amazonOrder));
                    }
                    else
                    {
                        _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders,null,
                                              "Amazon Order #" + rawOrder.AmazonOrderId+" uses different currency than current MrCMS Site.");
                        AmazonProgressBarHelper.Update(model.Task, "Import", "Skiping import of Amazon Order #" + rawOrder.AmazonOrderId + " which uses different currency than current MrCMS Site.",
                                               orders.Count, 0);
                    }
                }
            }

            return orders;
        }
    }
}