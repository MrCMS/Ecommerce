using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules
{
    public class OrderExists : IBulkShippingUpdateValidationRule
    {
        private readonly IOrderService _orderService;

        public OrderExists(IOrderService orderService)
        {
            _orderService = orderService; 
        }

        public IEnumerable<string> GetErrors(BulkShippingUpdateDataTransferObject item)
        {
            if (_orderService.Get(item.OrderId) == null)
                yield return string.Format("Order not present within the system.");
        }
    }
}