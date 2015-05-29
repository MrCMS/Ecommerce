using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderRefundService
    {
        void Add(OrderRefund orderRefund);
        void Delete(OrderRefund item);
    }
}