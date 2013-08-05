using System.Collections.Generic;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public interface IInventoryService
    {
        byte[] ExportLowStockReport(int treshold = 10);
        byte[] ExportStockReport();
        Dictionary<string, List<string>> BulkStockUpdate(Stream file);
    }
}