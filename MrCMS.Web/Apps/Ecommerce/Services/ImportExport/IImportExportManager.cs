using System.Collections.Generic;
using System.IO;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportExportManager
    {
        byte[] ExportProductsToExcel();
        Dictionary<string, List<string>> ImportProductsFromExcel(Stream file);

        byte[] ExportOrderToPdf(Order order);
    }
}