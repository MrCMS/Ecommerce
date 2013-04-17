using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderService
    {
        void PlaceOrder(CartModel cartModel);
        IPagedList<Order> GetPaged(int pageNum, int pageSize = 10);
        void Save(Order item);
        Order Get(int id);
    }
}