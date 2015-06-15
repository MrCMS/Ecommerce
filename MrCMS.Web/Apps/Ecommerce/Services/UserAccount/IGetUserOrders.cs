using MrCMS.Entities.People;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public interface IGetUserOrders
    {
        IPagedList<Order> Get(User user, int page = 1);
    }
}