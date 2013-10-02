using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPlacedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderService _orderService;

        public OrderPlacedController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public ActionResult Show(OrderPlaced page, Guid id)
        {
            var order = _orderService.GetByGuid(id);
            if (order != null)
            {
                ViewData["order"] = order;
                TempData["order"] = order;
                return View(page);
            }
            return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
        }
    }
}