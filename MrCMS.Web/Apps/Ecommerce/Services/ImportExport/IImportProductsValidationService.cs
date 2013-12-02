using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using OfficeOpenXml;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductsValidationService
    {
        Dictionary<string, List<string>> ValidateBusinessLogic(IEnumerable<ProductImportDataTransferObject> productsToImport);
        HashSet<ProductImportDataTransferObject> ValidateAndImportProductsWithVariants(ExcelPackage spreadsheet,
                                                                                    ref Dictionary<string, List<string>>
                                                                                        parseErrors);
        Dictionary<string, List<string>> ValidateImportFile(ExcelPackage spreadsheet);
    }
}