using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPlacedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderPlacedController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        public ActionResult Show(OrderPlaced page, Guid id)
        {
            ViewData["user-can-register"] = false;

            var order = _orderService.GetByGuid(id);
            if (order != null)
            {
                ViewData["order"] = order; 
                TempData["order"] = order;//required for Google Analytics

                if (_userService.GetUserByEmail(order.OrderEmail.Trim()) == null && CurrentRequestData.CurrentUser == null)
                    ViewData["user-can-register"] = true;

                return View(page);
            }
            return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
        }
    }
}