using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using NHibernate;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Entities.People;
using NHibernate.Criterion;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ISession _session;
        private readonly IOrderNoteService _orderNoteService;

        public OrderService(ISession session, IOrderNoteService orderNoteService)
        {
            _session = session;
            _orderNoteService = orderNoteService;
        }

        public IPagedList<Order> GetPaged(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Paged(pageNum, pageSize);
        }

        private IQueryOver<Order, Order> BaseQuery()
        {
            return
                _session.QueryOver<Order>()
                        .OrderBy(entry => entry.CreatedOn)
                        .Desc;
        }

        public void Save(Order item)
        {
            item.IsCancelled = false;
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public Order Get(int id)
        {
            return _session.QueryOver<Order>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public IPagedList<Order> GetOrdersByUser(User user, int pageNum, int pageSize = 10)
        {
            var id = user.Id;
            return _session.QueryOver<Order>().Where(x => x.User.Id == id).OrderBy(x => x.CreatedOn).Desc.Paged(pageNum, pageSize);
        }

        public IList<Order> GetOrdersByUser(User user)
        {
            var id = user.Id;
            var email = user.Email;
            return _session.QueryOver<Order>().Where(x => x.User.Id == id ||
                x.OrderEmail.IsInsensitiveLike(email, MatchMode.Exact)).OrderBy(x => x.CreatedOn).Desc.Cacheable().List();
        }

        public void Cancel(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as cancelled by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.IsCancelled = true;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderCancelled, OrderCancelledArgs>(new OrderCancelledArgs { Order = order });
        }

        public void MarkAsShipped(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as shipped by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.ShippingDate = CurrentRequestData.Now;
            order.ShippingStatus = ShippingStatus.Shipped;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderShipped, OrderShippedArgs>(new OrderShippedArgs { Order = order });
        }

        public void MarkAsPaid(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as paid by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.PaidDate = CurrentRequestData.Now;
            order.PaymentStatus = PaymentStatus.Paid;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderPaid, OrderPaidArgs>(new OrderPaidArgs { Order = order });
        }

        public void MarkAsVoided(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order payment marked as void by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.PaymentStatus = PaymentStatus.Voided;
            _session.Transact(session => session.Update(order));
        }

        public Order SetLastOrderUserIdByOrderId(int orderId)
        {
            var order = _session.Get<Order>(orderId);

            if (order == null) return null;

            order.User = CurrentRequestData.CurrentUser;
            _session.Transact(session => session.Update(order));
            return order;
        }

        public Order GetByGuid(Guid id)
        {
            return _session.QueryOver<Order>().Where(order => order.Guid == id).Take(1).Cacheable().SingleOrDefault();
        }

        public void Delete(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as deleted by {0}.",
                                                                       CurrentRequestData.CurrentUser.Name), order);
            _session.Transact(session => session.Delete(order));
        }
    }
}