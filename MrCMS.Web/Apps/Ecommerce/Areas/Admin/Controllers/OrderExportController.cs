using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderExportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderExportService _orderExportService;

        public OrderExportController(IOrderExportService orderExportService)
        {
            _orderExportService = orderExportService;
        }

        public ViewResult Index()
        {
            return View(_orderExportService.GetDefaultQuery());
        }

        public FileResult ToExcel(OrderExportQuery exportQuery)
        {
            return _orderExportService.ExportOrdersToExcel(exportQuery);
        }
    }
}