using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IStockReportAdminService
    {
        ExportStockReportResult ExportStockReport();
    }
}