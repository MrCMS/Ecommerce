using System.IO;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly IImportProductsValidationService _importProductsValidationService;
        private readonly IImportProductsService _importProductsService;
        private readonly IProductVariantService _productVariantService;

        public ImportExportManager(IImportProductsValidationService importProductsValidationService, IImportProductsService importProductsService, IProductVariantService productVariantService)
        {
            _importProductsValidationService = importProductsValidationService;
            _importProductsService = importProductsService;
            _productVariantService = productVariantService;
        }

        #region Import Products
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
            _importProductsService.ImportProductsFromDTOs(productsToImport);
            return new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Try and get data out of the spreadsheet into the DTOs with parse and type checks
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private List<ProductImportDataTransferObject> GetProductsFromSpreadSheet(ExcelPackage spreadsheet, out Dictionary<string, List<string>> parseErrors)
        {
            parseErrors = _importProductsValidationService.ValidateImportFile(spreadsheet);

            return _importProductsValidationService.ValidateAndImportProductsWithVariants(spreadsheet, ref parseErrors);
        }

        #endregion

        #region Export Products
        public byte[] ExportProductsToExcel()
        {
            using (var excelFile = new ExcelPackage())
            {
                var wsInfo = excelFile.Workbook.Worksheets.Add("Info");

                wsInfo.Cells["A1:D1"].Style.Font.Bold = true;
                wsInfo.Cells["A:D"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsInfo.Cells["A1"].Value = "MrCMS Version";
                wsInfo.Cells["B1"].Value = "Entity Type for Export";
                wsInfo.Cells["C1"].Value = "Export Date";
                wsInfo.Cells["D1"].Value = "Export Source";

                wsInfo.Cells["A2"].Value = MrCMSHtmlHelper.AssemblyVersion(null);
                wsInfo.Cells["B2"].Value = "Product";
                wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";
                wsInfo.Cells["C2"].Value = DateTime.UtcNow;
                wsInfo.Cells["D2"].Value = "MrCMS " + MrCMSHtmlHelper.AssemblyVersion(null);

                wsInfo.Cells["A:D"].AutoFitColumns();
                wsInfo.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                wsInfo.Cells["A4"].Value = "Please do not change any values inside this file.";

                var wsProducts = excelFile.Workbook.Worksheets.Add("Products");

                wsProducts.Cells["A1:AB1"].Style.Font.Bold = true;
                wsProducts.Cells["A:AB"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsProducts.Cells["A1"].Value = "Url (Must not be changed!)";
                wsProducts.Cells["B1"].Value = "Product Name";
                wsProducts.Cells["C1"].Value = "Description";
                wsProducts.Cells["D1"].Value = "SEO Title";
                wsProducts.Cells["E1"].Value = "SEO Description";
                wsProducts.Cells["F1"].Value = "SEO Keywords";
                wsProducts.Cells["G1"].Value = "Abstract";
                wsProducts.Cells["H1"].Value = "Brand";
                wsProducts.Cells["I1"].Value = "Categories";
                wsProducts.Cells["J1"].Value = "Specifications";
                wsProducts.Cells["K1"].Value = "Variant Name";
                wsProducts.Cells["L1"].Value = "Price";
                wsProducts.Cells["M1"].Value = "Previous Price";
                wsProducts.Cells["N1"].Value = "Tax Rate";
                wsProducts.Cells["O1"].Value = "Weight (g)";
                wsProducts.Cells["P1"].Value = "Stock";
                wsProducts.Cells["Q1"].Value = "Tracking Policy";
                wsProducts.Cells["R1"].Value = "SKU";
                wsProducts.Cells["S1"].Value = "Barcode";
                wsProducts.Cells["T1"].Value = "Option 1 Name";
                wsProducts.Cells["U1"].Value = "Option 1 Value";
                wsProducts.Cells["V1"].Value = "Option 2 Name";
                wsProducts.Cells["W1"].Value = "Option 2 Value";
                wsProducts.Cells["X1"].Value = "Option 3 Name";
                wsProducts.Cells["Y1"].Value = "Option 3 Value";
                wsProducts.Cells["Z1"].Value = "Image 1";
                wsProducts.Cells["AA1"].Value = "Image 2";
                wsProducts.Cells["AB1"].Value = "Image 3";

                var productVariants = _productVariantService.GetAll();

                for (var i = 0; i < productVariants.Count; i++)
                {
                    var rowId = i + 2;
                    wsProducts.Cells["A" + rowId].Value = productVariants[i].Product.UrlSegment;
                    wsProducts.Cells["B" + rowId].Value = productVariants[i].Product.Name;
                    wsProducts.Cells["C" + rowId].Value = productVariants[i].Product.BodyContent;
                    wsProducts.Cells["D" + rowId].Value = productVariants[i].Product.MetaTitle;
                    wsProducts.Cells["E" + rowId].Value = productVariants[i].Product.MetaDescription;
                    wsProducts.Cells["F" + rowId].Value = productVariants[i].Product.MetaKeywords;
                    wsProducts.Cells["G" + rowId].Value = productVariants[i].Product.Abstract;
                    if (productVariants[i].Product.Brand != null)
                        wsProducts.Cells["H" + rowId].Value = productVariants[i].Product.Brand.Name;
                    if (productVariants[i].Product.Categories.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.Categories)
                        {
                            wsProducts.Cells["I" + rowId].Value += item.Id + ";";
                        }
                    }
                    if (productVariants[i].Product.SpecificationValues.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.SpecificationValues)
                        {
                            wsProducts.Cells["J" + rowId].Value += item.SpecificationName + ":" + item.Value + ";";
                        }
                    }
                    wsProducts.Cells["K" + rowId].Value = productVariants[i].Name ?? String.Empty;
                    wsProducts.Cells["L" + rowId].Value = productVariants[i].BasePrice;
                    wsProducts.Cells["M" + rowId].Value = productVariants[i].PreviousPrice;
                    if (productVariants[i].TaxRate != null)
                        wsProducts.Cells["N" + rowId].Value = productVariants[i].TaxRate.Id;
                    wsProducts.Cells["O" + rowId].Value = productVariants[i].Weight;
                    wsProducts.Cells["P" + rowId].Value = productVariants[i].StockRemaining;
                    wsProducts.Cells["Q" + rowId].Value = productVariants[i].TrackingPolicy;
                    wsProducts.Cells["R" + rowId].Value = productVariants[i].SKU;
                    wsProducts.Cells["S" + rowId].Value = productVariants[i].Barcode;

                    for (var v = 0; v < productVariants[i].AttributeValues.OrderBy(x => x.ProductAttributeOption.DisplayOrder).Count(); v++)
                    {
                        if (v == 0)
                        {
                            wsProducts.Cells["T" + rowId].Value = productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsProducts.Cells["U" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 1)
                        {
                            wsProducts.Cells["V" + rowId].Value = productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsProducts.Cells["W" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 2)
                        {
                            wsProducts.Cells["X" + rowId].Value = productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsProducts.Cells["Y" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                    }

                    if (!productVariants[i].Product.Images.Any()) continue;
                    wsProducts.Cells["Z" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.First().FileUrl + "?update=no";
                    if (productVariants[i].Product.Images.Count() > 1)
                        wsProducts.Cells["AA" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.ToList()[1].FileUrl + "?update=no";
                    if (productVariants[i].Product.Images.Count() > 2)
                        wsProducts.Cells["AB" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.ToList()[2].FileUrl + "?update=no";
                }
                wsProducts.Cells["C:C"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsProducts.Cells["E:E"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsProducts.Cells["G:G"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsProducts.Cells["A:B"].AutoFitColumns();
                wsProducts.Cells["D:D"].AutoFitColumns();
                wsProducts.Cells["F:F"].AutoFitColumns();
                wsProducts.Cells["I:AB"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }
        #endregion
    }
}