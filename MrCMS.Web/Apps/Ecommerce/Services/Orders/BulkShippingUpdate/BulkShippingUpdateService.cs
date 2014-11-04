using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate
{
    public class BulkShippingUpdateService : IBulkShippingUpdateService
    {
        private readonly IOrderAdminService _orderService;
        private readonly IShippingMethodAdminService _shippingMethodAdminService;

        public BulkShippingUpdateService(IOrderAdminService orderService,IShippingMethodAdminService shippingMethodAdminService)
        {
            _orderService = orderService;
            _shippingMethodAdminService = shippingMethodAdminService;
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

        public void BulkShippingUpdate(BulkShippingUpdateDataTransferObject itemDto, ref int noOfUpdatedItems)
        {
            var order = _orderService.Get(itemDto.OrderId);

            if (order != null && !string.IsNullOrWhiteSpace(itemDto.ShippingMethod))
            {
                var shippingMethod =
                    _shippingMethodAdminService.GetAll().FirstOrDefault(info => info.Name == itemDto.ShippingMethod);
                if (shippingMethod != null)
                {
                    order.ShippingMethodName = shippingMethod.Name;
                    order.TrackingNumber = itemDto.TrackingNumber;
                    _orderService.MarkAsShipped(order);
                    noOfUpdatedItems++;
                }
            }
        }
    }
}