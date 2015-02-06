using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPdfController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderInvoiceService _orderInvoiceService;

        public OrderPdfController(IOrderInvoiceService orderInvoiceService)
        {
            _orderInvoiceService = orderInvoiceService;
        }

        public ActionResult ExportOrderToPdf(Order order)
        {
            var currentUser = CurrentRequestData.CurrentUser;

            if (currentUser == null)
                return Redirect("/");

            if (order.User.Id != currentUser.Id)
                return null;

            var file = _orderInvoiceService.GeneratePDF(order);
            return File(file, "application/pdf", "Order-" + order.Id + "-[" + CurrentRequestData.Now.ToString("dd-MM-yyyy hh-mm") + "].pdf");
        }

    }
}
