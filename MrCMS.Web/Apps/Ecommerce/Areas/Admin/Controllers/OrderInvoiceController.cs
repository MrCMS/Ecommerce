using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderInvoiceController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderInvoiceService _orderInvoiceService;

        public OrderInvoiceController(IOrderInvoiceService orderInvoiceService)
        {
            _orderInvoiceService = orderInvoiceService;
        }

        [HttpGet]
        public ActionResult Create(Order order)
        {
            try
            {
                byte[] file = _orderInvoiceService.GeneratePDF(order);
                return File(file, "application/pdf",
                    "Order-" + order.Id + "-[" + CurrentRequestData.Now.ToString("dd-MM-yyyy hh-mm") + "].pdf");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return RedirectToAction("Edit", "Order", new {id = order.Id});
            }
        }
    }
}