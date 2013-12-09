using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderLineService
    {
        void Save(OrderLine item);
        OrderLine Get(int id);
    }
}