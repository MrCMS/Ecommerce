using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using NHibernate;
using NHibernate.Criterion;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ISession _session;

        public OrderService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Order> GetPaged(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Paged(pageNum, pageSize);
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
            int id = user.Id;
            return _session.QueryOver<Order>()
                .Where(x => x.User.Id == id)
                .OrderBy(x => x.CreatedOn)
                .Desc.Paged(pageNum, pageSize);
        }

        public IList<Order> GetOrdersByUser(User user)
        {
            int id = user.Id;
            string email = user.Email;
            return _session.QueryOver<Order>().Where(x => x.User.Id == id ||
                                                          x.OrderEmail.IsInsensitiveLike(email, MatchMode.Exact))
                .OrderBy(x => x.CreatedOn)
                .Desc.Cacheable()
                .List();
        }

        public Order AssignUserToOrder(int orderId, User user)
        {
            var order = _session.Get<Order>(orderId);

            if (order == null) return null;

            order.User = user;
            _session.Transact(session => session.Update(order));
            return order;
        }

        public Order GetByGuid(Guid id)
        {
            return _session.QueryOver<Order>().Where(order => order.Guid == id).Take(1).Cacheable().SingleOrDefault();
        }

        private IQueryOver<Order, Order> BaseQuery()
        {
            return
                _session.QueryOver<Order>()
                    .OrderBy(entry => entry.CreatedOn)
                    .Desc;
        }
    }
}