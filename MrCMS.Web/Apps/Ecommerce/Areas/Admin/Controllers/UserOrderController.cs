using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class UserOrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IBelongToUserLookupService _belongToUserLookupService;

        public UserOrderController(IBelongToUserLookupService belongToUserLookupService)
        {
            _belongToUserLookupService = belongToUserLookupService;
        }

        [ChildActionOnly]
        public PartialViewResult List(User user)
        {
            var orders = _belongToUserLookupService.GetAll<Order>(user);
            return PartialView(orders);
        }

    }
}