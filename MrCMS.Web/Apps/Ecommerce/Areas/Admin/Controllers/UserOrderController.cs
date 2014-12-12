using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class UserOrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IUserService _userService;

        public UserOrderController(IUserService userService)
        {
            _userService = userService;
        }

        [ChildActionOnly]
        public PartialViewResult List(User user)
        {
            var orders = _userService.GetAll<Order>(user);
            return PartialView(orders);
        }

    }
}