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
    public class OrderNoteController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderNoteService _orderNoteService;
        private readonly IOrderService _orderService;

        public OrderNoteController(IOrderNoteService orderNoteService, IOrderService orderService)
        {
            _orderNoteService = orderNoteService;
            _orderService = orderService;
        }

        [HttpGet]
        public PartialViewResult Add(int orderID)
        {
            OrderNote orderNote = new OrderNote();
            orderNote.Order = _orderService.Get(orderID);
            orderNote.User = CurrentRequestData.CurrentUser;
            return PartialView(orderNote);
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(OrderNote orderNote)
        {
            orderNote.Order.OrderNotes.Add(orderNote);
            _orderNoteService.Add(orderNote);
            return RedirectToAction("Edit","Order", new { id = orderNote.Order.Id });
        }

        [HttpGet]
        public ViewResult Edit(OrderNote orderNote)
        {
            return View(orderNote);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(OrderNote orderNote)
        {
            orderNote.User = CurrentRequestData.CurrentUser;
            _orderNoteService.Save(orderNote);
            return RedirectToAction("Edit", "Order", new { id = orderNote.Order.Id });
        }

        [HttpGet]
        public PartialViewResult Delete(OrderNote orderNote)
        {
            return PartialView(orderNote);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(OrderNote orderNote)
        {
            _orderNoteService.Delete(orderNote);
            return RedirectToAction("Edit", "Order", new { id = orderNote.Order.Id });
        }
    }
}