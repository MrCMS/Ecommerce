using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public ViewResult Index(int page = 1)
        {
            return View(_orderService.GetPaged(page));
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            if (id > 0)
            {
                return View(_orderService.Get(id));
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(Order order)
        {
            order.User = CurrentRequestData.CurrentUser;
            _orderService.Save(order);
            return RedirectToAction("Index");
        }
    }
}