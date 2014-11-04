using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderService
    {
        IPagedList<Order> GetPaged(int pageNum, int pageSize = 10);
        void Save(Order item);
        Order Get(int id);
        IPagedList<Order> GetOrdersByUser(User user, int pageNum, int pageSize = 10);
        IList<Order> GetOrdersByUser(User user);
        Order AssignUserToOrder(int orderId, User user);
        Order GetByGuid(Guid id);
    }
}