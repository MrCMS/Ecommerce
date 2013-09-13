using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ImportAmazonOrderService : IImportAmazonOrderService
    {
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IAmazonApiService _amazonApiService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IOrderService _orderService;
        private readonly IValidateAmazonOrderService _validateAmazonOrderService;

        public ImportAmazonOrderService(IAmazonOrderService amazonOrderService, 
            IAmazonApiService amazonApiService,
            IAmazonLogService amazonLogService, IOrderService orderService, 
            IValidateAmazonOrderService validateAmazonOrderService)
        {
            _amazonOrderService = amazonOrderService;
            _amazonApiService = amazonApiService;
            _amazonLogService = amazonLogService;
            _orderService = orderService;
            _validateAmazonOrderService = validateAmazonOrderService;
        }

        public void ImportOrders(AmazonSyncModel model, ICollection<AmazonOrder> orders)
        {
            if (orders.Any())
            {
                _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders, null,null,null, "Importing Amazon Orders");
                AmazonProgressBarHelper.Update(model.TaskId.Value, "Import", "Importing Amazon Orders", orders.Count, 0);

                var cnt = 0;
                foreach (var order in orders)
                {
                    _amazonLogService.Add(AmazonLogType.Orders,
                                          order.Id > 0 ? AmazonLogStatus.Update : AmazonLogStatus.Insert,
                                          AmazonApiSection.Orders, order,null,null,"Importing Amazon Order #"+order.AmazonOrderId +" and mapping to MrCMS order");
                    AmazonProgressBarHelper.Update(model.TaskId.Value, "Import", "Importing Amazon Order #" + order.AmazonOrderId, orders.Count, cnt+1);

                    _orderService.Save(order.Order);
                    _amazonOrderService.Save(order);
                }

                AmazonProgressBarHelper.Update(model.TaskId.Value, "Import", "Successfully imported Amazon Orders",
                                               orders.Count, 0);
            }
            else
            {
                AmazonProgressBarHelper.Update(model.TaskId.Value, "Import", "No orders for import",
                                               null, null);
            }
        }

        public List<AmazonOrder> GetOrdersFromAmazon(AmazonSyncModel model)
        {
            var orders = new List<AmazonOrder>();

            _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, null, null, "Getting Orders From Amazon");
            AmazonProgressBarHelper.Update(model.TaskId.Value, "Import", "Getting Orders From Amazon", 100, 0);

            var rawOrders = _amazonApiService.ListOrders(model);
            
            if (rawOrders != null)
            {
                foreach (var rawOrder in rawOrders)
                {
                    var amazonOrder = _validateAmazonOrderService.GetAmazonOrder(rawOrder);

                    if (amazonOrder != null)
                    {
                        _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders, null,
                                              "Getting Items From Amazon for Order #" + rawOrder.AmazonOrderId);

                        var rawOrderItems = _amazonApiService.ListOrderItems(model, rawOrder.AmazonOrderId);

                        orders.Add(_validateAmazonOrderService.SetAmazonOrderItems(rawOrder, rawOrderItems, amazonOrder));
                    }
                    else
                    {
                        _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, AmazonApiSection.Orders,null,
                                              "Amazon Order #" + rawOrder.AmazonOrderId+" uses different currency than current MrCMS Site.");
                        AmazonProgressBarHelper.Update(model.TaskId.Value, "Import", "Skiping import of Amazon Order #" + rawOrder.AmazonOrderId + " which uses different currency than current MrCMS Site.",
                                               orders.Count, 0);
                    }
                }
            }

            return orders;
        }
    }
}