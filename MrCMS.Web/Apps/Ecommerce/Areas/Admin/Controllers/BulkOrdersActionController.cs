using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ionic.Zip;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class BulkOrdersActionController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderInvoiceService _orderInvoiceService;
        private readonly IOrderService _orderService;
        private readonly IOrderAdminService _orderAdminService;
        private readonly IBulkOrdersAdminService _bulkOrdersAdminService;

        public BulkOrdersActionController(IOrderInvoiceService orderInvoiceService, IOrderService orderService,
            IOrderAdminService orderAdminService, IBulkOrdersAdminService bulkOrdersAdminService)
        {
            _orderInvoiceService = orderInvoiceService;
            _orderService = orderService;
            _orderAdminService = orderAdminService;
            _bulkOrdersAdminService = bulkOrdersAdminService;
        }

        public ViewResult MarkOrdersAsShippedForm(SelectedOrdersViewModel model)
        {
            var shippedModel = _bulkOrdersAdminService.GetModel(model);
            return View(shippedModel);
        }

        [ActionName("MarkOrdersAsShipped")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult MarkOrdersAsShipped(MarkOrdersAsShippedViewModel model)
        {
            if (model.Orders.Any())
            {
                foreach (var order in model.Orders)
                    _orderAdminService.MarkAsShipped(order);

                TempData["orders-shipped"] = true;
            }
            return RedirectToAction("Index", "Order", new { page = model.Page });
        }

        public ViewResult CreatePickingList(SelectedOrdersViewModel model)
        {
            var pickingListModel = _bulkOrdersAdminService.GetPickingList(model);
            return View(pickingListModel);
        }

        public FileResult CreateInvoices(string orders)
        {
            //Define file Type
            string fileType = "application/octet-stream";

            //Define Output Memory Stream
            var outputStream = new MemoryStream();

            //Create object of ZipFile library
            using (var zipFile = new ZipFile())
            {
                var orderIds = orders.Split(',').Select(int.Parse).ToList();
                if (orderIds.Any())
                {
                    foreach (var orderId in orderIds)
                    {
                        var order = _orderService.Get(orderId);
                        byte[] bytes = _orderInvoiceService.GeneratePDF(order);
                        zipFile.AddEntry("Order-" + orderId + ".pdf", bytes);
                    }
                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=Orders.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }
    }
}