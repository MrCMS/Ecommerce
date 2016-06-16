using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderNoteService
    {
        void Save(OrderNote item);
        void Delete(OrderNote item);
        void AddOrderNoteAudit(string note, Order order);
    }
}