using System.Collections.Generic;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public interface IInventoryService
    {
        Dictionary<string, List<string>> BulkStockUpdate(Stream file);
        byte[] ExportLowStockReport(int threshold);
        byte[] ExportStockReport();
    }
}