using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ILowStockReportAdminService
    {
        IPagedList<ProductVariant> Search(LowStockReportSearchModel searchModel);
        ExportStockReportResult ExportLowStockReport(LowStockReportSearchModel searchModel);
    }
}