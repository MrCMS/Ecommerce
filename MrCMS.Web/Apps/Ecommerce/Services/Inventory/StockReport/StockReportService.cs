using System.IO;
using CsvHelper;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.StockReport
{
    public class StockReportService : IStockReportService
    {
        private readonly IProductVariantService _productVariantService;

        public StockReportService(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        public byte[] GenerateLowStockReport(int treshold=10)
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
        public byte[] GenerateStockReport()
        {
            var items = _productVariantService.GetAllVariantsForStockReport();

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
            if (item.Product.Published)
            {
                w.WriteField(item.FullName);
                w.WriteField(item.SKU);
                w.WriteField(item.StockRemaining);
                w.NextRecord();
            }
        }
    }
}