using System.IO;
using CsvHelper;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductVariantService _productVariantService;

        public InventoryService(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        public byte[] ExportLowStockReport(int treshold=10)
        {
            var items = _productVariantService.GetAllVariantsWithLowStock(treshold);
            
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var w = new CsvWriter(sw))
            {
                WriteHeaders(w);

                foreach (var item in items)
                {
                    WriteProduct(w, item);
                }

                sw.Flush();
                var file = ms.ToArray();
                sw.Close();
                
                return file;
            }
        }

        private void WriteHeaders(CsvWriter w)
        {
            w.WriteField("Name");
            w.WriteField("SKU");
            w.WriteField("Stock Remaining");
            w.NextRecord();
        }

        private void WriteProduct(CsvWriter w, ProductVariant item)
        {
            w.WriteField(item.Name);
            w.WriteField(item.SKU);
            w.WriteField(item.StockRemaining);
            w.NextRecord();
        }
    }
}