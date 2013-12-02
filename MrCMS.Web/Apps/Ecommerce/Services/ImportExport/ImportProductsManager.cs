using System.IO;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductsManager : IImportProductsManager
    {
        private readonly IImportProductsValidationService _importProductsValidationService;
        private readonly IImportProductsService _importProductsService;

        public ImportProductsManager(IImportProductsValidationService importProductsValidationService,
                                   IImportProductsService importProductsService)
        {
            _importProductsValidationService = importProductsValidationService;
            _importProductsService = importProductsService;
        }

        public Dictionary<string, List<string>> ImportProductsFromExcel(Stream file)
        {
            var spreadsheet = new ExcelPackage(file);

            Dictionary<string, List<string>> parseErrors;
            var productsToImport = GetProductsFromSpreadSheet(spreadsheet, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            var businessLogicErrors = _importProductsValidationService.ValidateBusinessLogic(productsToImport);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            _importProductsService.Initialize();
            _importProductsService.ImportProductsFromDTOs(productsToImport);
            return new Dictionary<string, List<string>>();
        }

        private HashSet<ProductImportDataTransferObject> GetProductsFromSpreadSheet(ExcelPackage spreadsheet,
                                                                                 out Dictionary<string, List<string>>
                                                                                     parseErrors)
        {
            parseErrors = _importProductsValidationService.ValidateImportFile(spreadsheet);

            return _importProductsValidationService.ValidateAndImportProductsWithVariants(spreadsheet, ref parseErrors);
        }
    }
}