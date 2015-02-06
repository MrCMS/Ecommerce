using System.Collections.Generic;
using System.IO;
using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using OfficeOpenXml;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductsManager : IImportProductsManager
    {
        private readonly IImportProductsService _importProductsService;
        private readonly IImportProductsValidationService _importProductsValidationService;

        public ImportProductsManager(IImportProductsValidationService importProductsValidationService,
            IImportProductsService importProductsService)
        {
            _importProductsValidationService = importProductsValidationService;
            _importProductsService = importProductsService;
        }

        public ImportProductsResult ImportProductsFromExcel(Stream file, bool autoStart = true)
        {
            var spreadsheet = new ExcelPackage(file);

            Dictionary<string, List<string>> parseErrors;
            HashSet<ProductImportDataTransferObject> productsToImport = GetProductsFromSpreadSheet(spreadsheet,
                out parseErrors);
            if (parseErrors.Any())
                return ImportProductsResult.Failure(GetErrors(parseErrors));
            Dictionary<string, List<string>> businessLogicErrors =
                _importProductsValidationService.ValidateBusinessLogic(productsToImport);
            if (businessLogicErrors.Any())
                ImportProductsResult.Failure(GetErrors(businessLogicErrors));
            Batch batch = _importProductsService.CreateBatch(productsToImport);
            //_importProductsService.Initialize();
            //_importProductsService.ImportProductsFromDTOs(productsToImport);
            return ImportProductsResult.Successful(batch);
        }

        private static List<string> GetErrors(Dictionary<string, List<string>> parseErrors)
        {
            return parseErrors.SelectMany(pair => pair.Value.Select(value => pair.Key + ": " + value)).ToList();
        }

        private HashSet<ProductImportDataTransferObject> GetProductsFromSpreadSheet(ExcelPackage spreadsheet,
            out Dictionary<string, List<string>>
                parseErrors)
        {
            parseErrors = _importProductsValidationService.ValidateImportFile(spreadsheet);

            return _importProductsValidationService.ValidateAndImportProductsWithVariants(spreadsheet, ref parseErrors);
        }
    }

    public class ImportProductsResult
    {
        private ImportProductsResult()
        {
            Errors = new List<string>();
        }

        public Batch Batch { get; private set; }
        public List<string> Errors { get; private set; }

        public bool Success
        {
            get { return Batch != null; }
        }

        public static ImportProductsResult Successful(Batch batch)
        {
            return new ImportProductsResult {Batch = batch};
        }

        public static ImportProductsResult Failure(List<string> errors)
        {
            return new ImportProductsResult {Errors = errors};
        }
    }
}