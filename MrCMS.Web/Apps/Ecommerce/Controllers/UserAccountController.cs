using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderService _orderService;

        public UserAccountController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public ActionResult UserAccountOrders(int page = 1)
        {
            var user = CurrentRequestData.CurrentUser;
            if (user != null)
            {
                var ordersByUser = _orderService.GetOrdersByUser(user, page);
                var model = new UserAccountOrdersModel(new PagedList<Order>(ordersByUser, ordersByUser.PageNumber, ordersByUser.PageSize), user.Id);
                return View(model);
            }
            return Redirect(UniquePageHelper.GetUrl<LoginPage>());
        }
    }
}