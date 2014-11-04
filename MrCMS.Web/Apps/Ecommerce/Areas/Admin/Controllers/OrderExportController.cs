using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderExportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IExportOrdersService _exportOrdersService;

        public OrderExportController(IExportOrdersService exportOrdersService)
        {
            _exportOrdersService = exportOrdersService;
        }

        [HttpGet]
        public ActionResult ToPDF(Order order)
        {
            try
            {
                var file = _exportOrdersService.ExportOrderToPdf(order);
                return File(file, "application/pdf",
                    "Order-" + order.Id + "-[" + CurrentRequestData.Now.ToString("dd-MM-yyyy hh-mm") + "].pdf");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            }
        }
    }
}