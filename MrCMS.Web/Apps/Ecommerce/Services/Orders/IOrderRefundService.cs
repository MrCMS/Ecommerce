using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderRefundService
    {
        IList<OrderRefund> GetAll();
        void Add(OrderRefund orderRefund, Order order);
        void Delete(OrderRefund item);
    }
}