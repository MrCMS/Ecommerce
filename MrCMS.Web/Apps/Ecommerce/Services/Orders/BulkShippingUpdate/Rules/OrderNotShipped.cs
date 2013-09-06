using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules
{
    public class OrderNotShipped : IBulkShippingUpdateValidationRule
    {
        private readonly IOrderService _orderService;

        public OrderNotShipped(IOrderService orderService)
        {
            _orderService = orderService; 
        }

        public IEnumerable<string> GetErrors(BulkShippingUpdateDataTransferObject item)
        {
            var order = _orderService.Get(item.OrderId);
            if (order != null && order.ShippingStatus==ShippingStatus.Shipped)
                yield return string.Format("Order with Id: {0} is already shipped.", item.OrderId);
        }
    }
}