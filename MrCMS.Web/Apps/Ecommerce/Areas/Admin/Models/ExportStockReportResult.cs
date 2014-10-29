using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class ExportStockReportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public FileResult FileResult { get; set; }
    }
}