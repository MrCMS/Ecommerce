using System.Web.Mvc;
using MrCMS.Entities.People;
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
            User currentUser = CurrentRequestData.CurrentUser;

            if (currentUser == null)
                return Redirect("/");

            if (order.User.Id != currentUser.Id)
                return null;

            byte[] file = _orderInvoiceService.GeneratePDF(order);
            return File(file, "application/pdf",
                "Order-" + order.Id + "-[" + CurrentRequestData.Now.ToString("dd-MM-yyyy hh-mm") + "].pdf");
        }
    }
}