using System.Collections.Generic;
using System.IO;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public interface IStockAdminService
    {
        Dictionary<string, List<string>> BulkStockUpdate(Stream file);
        byte[] ExportLowStockReport(int threshold);
        byte[] ExportStockReport();
        IPagedList<ProductVariant> GetAllVariantsWithLowStock(int threshold, int page);
        void Update(ProductVariant productVariant);
    }
}