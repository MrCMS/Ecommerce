using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IOrderExportService
    {
        FileResult ExportOrdersToExcel(OrderExportQuery exportQuery);
        OrderExportQuery GetDefaultQuery();
    }
}