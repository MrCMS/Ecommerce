using System.Collections.Generic;
using System.IO;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.StockReport;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IBulkStockUpdateValidationService _bulkStockUpdateValidationService;
        private readonly IBulkStockUpdateService _bulkStockUpdateService;
        private readonly IStockReportService _stockReportService;

        public InventoryService(IBulkStockUpdateValidationService bulkStockUpdateValidationService,
                                   IBulkStockUpdateService bulkStockUpdateService,
                                    IStockReportService stockReportService)
        {
            _bulkStockUpdateValidationService = bulkStockUpdateValidationService;
            _bulkStockUpdateService = bulkStockUpdateService;
            _stockReportService = stockReportService;
        }

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

        public byte[] ExportLowStockReport(int threshold)
        {
            return _stockReportService.GenerateLowStockReport(threshold);
        }

        public byte[] ExportStockReport()
        {
            return _stockReportService.GenerateStockReport();
        }
    }
}