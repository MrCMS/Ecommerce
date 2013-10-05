using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate
{
    public class BulkShippingUpdateService : IBulkShippingUpdateService
    {
        private readonly IOrderService _orderService;
        private readonly IShippingMethodManager _shippingMethodManager;

        public BulkShippingUpdateService(IOrderService orderService, IShippingMethodManager shippingMethodManager)
        {
            _orderService = orderService;
            _shippingMethodManager = shippingMethodManager;
        }

        public int BulkShippingUpdateFromDTOs(IEnumerable<BulkShippingUpdateDataTransferObject> items)
        {
            var noOfUpdatedItems = 0;
            foreach (var dataTransferObject in items)
            {
                BulkShippingUpdate(dataTransferObject, ref noOfUpdatedItems);
            }

            return noOfUpdatedItems;
        }

        public Order BulkShippingUpdate(BulkShippingUpdateDataTransferObject itemDto, ref int noOfUpdatedItems)
        {
            var item = _orderService.Get(itemDto.OrderId);

            if (item != null && !string.IsNullOrWhiteSpace(itemDto.ShippingMethod))
            {
                var shippingMethod=_shippingMethodManager.GetByName(itemDto.ShippingMethod);
                if (shippingMethod != null)
                {
                    item.ShippingMethod = shippingMethod;
                    item.TrackingNumber = itemDto.TrackingNumber;
                    _orderService.MarkAsShipped(item);
                    noOfUpdatedItems++;
                }
                return item;
            }
            return new Order();
        }
    }
}