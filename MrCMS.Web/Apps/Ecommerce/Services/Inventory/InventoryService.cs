using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IBulkStockUpdateValidationService _bulkStockUpdateValidationService;
        private readonly IBulkStockUpdateService _bulkStockUpdateService;
        private readonly IProductVariantService _productVariantService;

        public InventoryService(IBulkStockUpdateValidationService bulkStockUpdateValidationService,
                                   IBulkStockUpdateService bulkStockUpdateService,
                                   IProductVariantService productVariantService)
        {
            _bulkStockUpdateValidationService = bulkStockUpdateValidationService;
            _bulkStockUpdateService = bulkStockUpdateService;
            _productVariantService = productVariantService;
        }

        #region Bulk Stock Update
        public Dictionary<string, List<string>> BulkStockUpdate(Stream file)
        {
            Dictionary<string, List<string>> parseErrors;
            var items = GetProductVariantsFromFile(file, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            var businessLogicErrors = _bulkStockUpdateValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            var noOfUpdatedItems = _bulkStockUpdateService.BulkStockUpdateFromDTOs(items);
            return new Dictionary<string, List<string>>() { { "success", new List<string>() { noOfUpdatedItems>0?noOfUpdatedItems.ToString()+" items were successfully updated.":"No items were updated." } } };
        }

        private List<BulkStockUpdateDataTransferObject> GetProductVariantsFromFile(Stream file, out Dictionary<string, List<string>>parseErrors)
        {
            parseErrors=new Dictionary<string, List<string>>();
            return _bulkStockUpdateValidationService.ValidateAndBulkStockUpdateProductVariants(file, ref parseErrors);
        }
        #endregion

        #region Stock Reports
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
        public byte[] ExportStockReport()
        {
            var items = _productVariantService.GetAll();

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
        #endregion
    }
}