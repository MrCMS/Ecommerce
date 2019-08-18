using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class OrderAdminService : IOrderAdminService
    {
        private readonly IOrderSearchService _orderSearcService;
        private readonly ISession _session;
        private readonly IShippingMethodAdminService _shippingMethodAdminService;

        public OrderAdminService(IOrderSearchService orderSearcService, ISession session, IShippingMethodAdminService shippingMethodAdminService)
        {
            _orderSearcService = orderSearcService;
            _session = session;
            _shippingMethodAdminService = shippingMethodAdminService;
        }

        public IPagedList<Order> Search(OrderSearchModel model)
        {
            return _orderSearcService.SearchOrders(model);
        }

        public Order Get(int id)
        {
            return _session.Get<Order>(id);
        }

        public void Cancel(Order order)
        {
            order.IsCancelled = true;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderCancelled, OrderCancelledArgs>(new OrderCancelledArgs { Order = order });
        }

        public List<SelectListItem> GetShippingStatusOptions()
        {
            return
                Enum.GetValues(typeof(ShippingStatus))
                    .Cast<ShippingStatus>()
                    .BuildSelectItemList(item => item.GetDescription(), item => item.ToString(), emptyItemText: "All");
        }

        public List<SelectListItem> GetPaymentStatusOptions()
        {
            return
                Enum.GetValues(typeof(PaymentStatus))
                    .Cast<PaymentStatus>()
                    .BuildSelectItemList(item => item.GetDescription(), item => item.ToString(), emptyItemText: "All");
        }

        public List<SelectListItem> GetShippingMethodOptions()
        {
            return _shippingMethodAdminService.GetAll().Where(info => info.Enabled)
                .BuildSelectItemList(info => info.DisplayName, info => info.Name, emptyItem: null);
        }

        public void MarkAsShipped(Order order)
        {
            order.ShippingDate = CurrentRequestData.Now;
            order.ShippingStatus = ShippingStatus.Shipped;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderShipped, OrderShippedArgs>(new OrderShippedArgs { Order = order });
        }

        public void MarkAsPaid(Order order)
        {

            order.PaidDate = CurrentRequestData.Now;
            order.PaymentStatus = PaymentStatus.Paid;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderPaid, OrderPaidArgs>(new OrderPaidArgs { Order = order });
        }

        public void MarkAsVoided(Order order)
        {
            order.PaymentStatus = PaymentStatus.Voided;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderVoided, OrderVoidedArgs>(new OrderVoidedArgs { Order = order });
        }

        public void SetTrackingNumber(Order order)
        {
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderTrackingNumberChanged, OrderTrackingNumberChangedArgs>(new OrderTrackingNumberChangedArgs{ Order = order });
        }

        public void Delete(Order order)
        {
            _session.Transact(session => session.Delete(order));
        }
    }
}