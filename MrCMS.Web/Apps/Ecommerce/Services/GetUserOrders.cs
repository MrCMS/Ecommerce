using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetUserOrders : GetWidgetModelBase<UserOrdersList>
    {
        private readonly IGetCurrentUser _currentUser;
        private readonly IOrderService _orderService;

        public GetUserOrders(IGetCurrentUser currentUser, IOrderService orderService)
        {
            _currentUser = currentUser;
            _orderService = orderService;
        }

        public override object GetModel(UserOrdersList widget)
        {
            var user = _currentUser.Get();
            if (user == null) return null;
            var ordersByUser = _orderService.GetOrdersByUser(user, 1);
            return new UserAccountOrdersModel(new PagedList<Order>(ordersByUser, ordersByUser.PageNumber, ordersByUser.PageSize), user.Id);
        }
    }
}