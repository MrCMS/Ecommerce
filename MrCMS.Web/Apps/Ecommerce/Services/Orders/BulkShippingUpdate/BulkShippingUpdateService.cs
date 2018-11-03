using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate
{
    public class BulkShippingUpdateService : IBulkShippingUpdateService
    {
        private readonly IOrderAdminService _orderService;
        private readonly IShippingMethodAdminService _shippingMethodAdminService;
        private readonly ISession _session;

        public BulkShippingUpdateService(IOrderAdminService orderService,
            IShippingMethodAdminService shippingMethodAdminService, ISession session)
        {
            _orderService = orderService;
            _shippingMethodAdminService = shippingMethodAdminService;
            _session = session;
        }

        public int BulkShippingUpdateFromDTOs(IEnumerable<BulkShippingUpdateDataTransferObject> items, bool sendEmails)
        {
            int noOfUpdatedItems = 0;
            _session.Transact(session =>
            {
                var shippingMethodInfos = _shippingMethodAdminService.GetAll();
                using (new BulkShippingUpdateEmailDisabler(sendEmails))
                {
                    foreach (BulkShippingUpdateDataTransferObject dataTransferObject in items)
                    {
                        Order order = _orderService.Get(dataTransferObject.OrderId);

                        if (order == null /*|| string.IsNullOrWhiteSpace(dataTransferObject.ShippingMethod)*/)
                            continue;

                        if (order.OrderStatus == OrderStatus.Shipped || order.OrderStatus == OrderStatus.Complete)
                            continue;

                        if (!string.IsNullOrWhiteSpace(dataTransferObject.ShippingMethod))
                        {
                            ShippingMethodInfo shippingMethod = shippingMethodInfos.FirstOrDefault(info => (info.DisplayName == dataTransferObject.ShippingMethod || info.Name == dataTransferObject.ShippingMethod));
                            if (shippingMethod != null)
                                order.ShippingMethodName = shippingMethod.DisplayName;
                        }
                        
                        order.TrackingNumber = dataTransferObject.TrackingNumber;
                        _orderService.MarkAsShipped(order);
                        noOfUpdatedItems++;
                    }
                }
            });

            return noOfUpdatedItems;
        }
    }

    public class BulkShippingUpdateEmailDisabler : IDisposable
    {
        private readonly IDisposable _emailDisabler;
        public BulkShippingUpdateEmailDisabler(bool sendEmails)
        {
            if (!sendEmails)
                _emailDisabler = EventContext.Instance.Disable<SendOrderShippedEmailToCustomer>();
        }

        public void Dispose()
        {
            if (_emailDisabler != null)
                _emailDisabler.Dispose();
        }
    }
}