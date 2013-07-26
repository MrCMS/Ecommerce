using System.IO;
using MrCMS.Helpers;
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

        public ImportExportManager(IImportProductsValidationService importProductsValidationService, 
            IImportProductsService importProductsService, 
            IProductVariantService productVariantService)
        {
            _importProductsValidationService = importProductsValidationService;
            _importProductsService = importProductsService;
            _productVariantService = productVariantService;
        }

        #region Products
        /// <summary>
        /// Import Products From Excel
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Export Products To Excel
        /// </summary>
        /// <returns></returns>
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

                var wsItems = excelFile.Workbook.Worksheets.Add("Items");

                wsItems.Cells["A1:AE1"].Style.Font.Bold = true;
                wsItems.Cells["A:AE"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsItems.Cells["A1"].Value = "Url (Must not be changed!)";
                wsItems.Cells["B1"].Value = "Product Name";
                wsItems.Cells["C1"].Value = "Description";
                wsItems.Cells["D1"].Value = "SEO Title";
                wsItems.Cells["E1"].Value = "SEO Description";
                wsItems.Cells["F1"].Value = "SEO Keywords";
                wsItems.Cells["G1"].Value = "Abstract";
                wsItems.Cells["H1"].Value = "Brand";
                wsItems.Cells["I1"].Value = "Categories";
                wsItems.Cells["J1"].Value = "Specifications";
                wsItems.Cells["K1"].Value = "Variant Name";
                wsItems.Cells["L1"].Value = "Price";
                wsItems.Cells["M1"].Value = "Previous Price";
                wsItems.Cells["N1"].Value = "Tax Rate";
                wsItems.Cells["O1"].Value = "Weight (g)";
                wsItems.Cells["P1"].Value = "Stock";
                wsItems.Cells["Q1"].Value = "Tracking Policy";
                wsItems.Cells["R1"].Value = "SKU";
                wsItems.Cells["S1"].Value = "Barcode";
                wsItems.Cells["T1"].Value = "Option 1 Name";
                wsItems.Cells["U1"].Value = "Option 1 Value";
                wsItems.Cells["V1"].Value = "Option 2 Name";
                wsItems.Cells["W1"].Value = "Option 2 Value";
                wsItems.Cells["X1"].Value = "Option 3 Name";
                wsItems.Cells["Y1"].Value = "Option 3 Value";
                wsItems.Cells["Z1"].Value = "Image 1";
                wsItems.Cells["AA1"].Value = "Image 2";
                wsItems.Cells["AB1"].Value = "Image 3";
                wsItems.Cells["AC1"].Value = "Price Breaks";
                wsItems.Cells["AD1"].Value = "Url History";
                wsItems.Cells["AE1"].Value = "Publish Date";

                var productVariants = _productVariantService.GetAll();

                for (var i = 0; i < productVariants.Count; i++)
                {
                    var rowId = i + 2;
                    wsItems.Cells["A" + rowId].Value = productVariants[i].Product.UrlSegment;
                    wsItems.Cells["A" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["B" + rowId].Value = productVariants[i].Product.Name;
                    wsItems.Cells["B" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["C" + rowId].Value = productVariants[i].Product.BodyContent;
                    wsItems.Cells["C" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    wsItems.Cells["D" + rowId].Value = productVariants[i].Product.MetaTitle;
                    wsItems.Cells["E" + rowId].Value = productVariants[i].Product.MetaDescription;
                    wsItems.Cells["E" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    wsItems.Cells["F" + rowId].Value = productVariants[i].Product.MetaKeywords;
                    wsItems.Cells["G" + rowId].Value = productVariants[i].Product.Abstract;
                    wsItems.Cells["G" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    if (productVariants[i].Product.Brand != null)
                        wsItems.Cells["H" + rowId].Value = productVariants[i].Product.Brand.Name;
                    if (productVariants[i].Product.Categories.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.Categories)
                        {
                            wsItems.Cells["I" + rowId].Value += item.Id + ";";
                        }
                    }
                    if (productVariants[i].Product.SpecificationValues.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.SpecificationValues)
                        {
                            wsItems.Cells["J" + rowId].Value += item.SpecificationName + ":" + item.Value + ";";
                        }
                        wsItems.Cells["J" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    wsItems.Cells["K" + rowId].Value = productVariants[i].Name ?? String.Empty;
                    wsItems.Cells["L" + rowId].Value = productVariants[i].BasePrice;
                    wsItems.Cells["M" + rowId].Value = productVariants[i].PreviousPrice;
                    if (productVariants[i].TaxRate != null)
                        wsItems.Cells["N" + rowId].Value = productVariants[i].TaxRate.Id;
                    wsItems.Cells["O" + rowId].Value = productVariants[i].Weight;
                    wsItems.Cells["P" + rowId].Value = productVariants[i].StockRemaining;
                    wsItems.Cells["Q" + rowId].Value = productVariants[i].TrackingPolicy;
                    wsItems.Cells["R" + rowId].Value = productVariants[i].SKU;
                    wsItems.Cells["S" + rowId].Value = productVariants[i].Barcode;

                    for (var v = 0; v < productVariants[i].AttributeValues.OrderBy(x => x.ProductAttributeOption.DisplayOrder).Count(); v++)
                    {
                        if (v == 0)
                        {
                            wsItems.Cells["T" + rowId].Value = productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["U" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 1)
                        {
                            wsItems.Cells["V" + rowId].Value = productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["W" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 2)
                        {
                            wsItems.Cells["X" + rowId].Value = productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["Y" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                    }

                    if (productVariants[i].PriceBreaks.Count > 0)
                    {
                        foreach (var item in productVariants[i].PriceBreaks)
                        {
                            wsItems.Cells["AC" + rowId].Value += item.Quantity + ":" + item.Price.ToString("#.##") + ";";
                        }
                        wsItems.Cells["AC" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    if (productVariants[i].Product.Urls.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.Urls)
                        {
                            wsItems.Cells["AD" + rowId].Value += item.UrlSegment + ",";
                        }
                        wsItems.Cells["AD" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    if (productVariants[i].Product.Published)
                        wsItems.Cells["AE" + rowId].Value = productVariants[i].Product.PublishOn;
                    wsItems.Cells["AE" + rowId].Style.Numberformat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";

                    if (!productVariants[i].Product.Images.Any()) continue;
                    wsItems.Cells["Z" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.First().FileUrl + "?update=no";
                    wsItems.Cells["Z" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    if (productVariants[i].Product.Images.Count() > 1)
                    {
                        wsItems.Cells["AA" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.ToList()[1].FileUrl + "?update=no";
                        wsItems.Cells["AA" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    if (productVariants[i].Product.Images.Count() > 2)
                    {
                        wsItems.Cells["AB" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                                            productVariants[i].Product.Images.ToList()[2].FileUrl +
                                                            "?update=no";
                        wsItems.Cells["AB" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                }
                wsItems.Cells["A:B"].AutoFitColumns();
                wsItems.Cells["D:D"].AutoFitColumns();
                wsItems.Cells["F:F"].AutoFitColumns();
                wsItems.Cells["I:AE"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }
        #endregion

        #region Google Base
        /// <summary>
        /// Export Products To Google Base
        /// </summary>
        /// <returns></returns>
        public byte[] ExportProductsToGoogleBase()
        {
            return null;
        }
        #endregion
    }
}