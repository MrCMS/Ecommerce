using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Pages;
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

        public ActionResult UserAccountOrders(int page=1)
        {
            var user = CurrentRequestData.CurrentUser;
            if (user != null)
            {
                var model = new UserAccountOrdersModel(_orderService.GetOrdersByUser(user,page), user.Id);
                return View(model);
            }
            return Redirect(UniquePageHelper.GetUrl<LoginPage>());
        }
    }
}