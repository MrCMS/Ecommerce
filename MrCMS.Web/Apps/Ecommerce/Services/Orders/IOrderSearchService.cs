using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderSearchService
    {
        IPagedList<Order> SearchOrders(OrderSearchModel model);
    }
}