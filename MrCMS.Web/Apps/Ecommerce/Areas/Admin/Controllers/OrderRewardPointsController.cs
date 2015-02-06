using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderRewardPointsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderRewardAdminService _orderRewardAdminService;

        public OrderRewardPointsController(IOrderRewardAdminService orderRewardAdminService)
        {
            _orderRewardAdminService = orderRewardAdminService;
        }

        [ChildActionOnly]
        public PartialViewResult Details(Order order)
        {
            return PartialView(_orderRewardAdminService.GetOrderRewardPointsUsage(order));
        }
    }
}