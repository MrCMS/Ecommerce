using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Linq;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ExportProductsManager : IExportProductsManager
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity;

        public ExportProductsManager(IProductVariantService productVariantService, IGetStockRemainingQuantity getStockRemainingQuantity)
        {
            _productVariantService = productVariantService;
            _getStockRemainingQuantity = getStockRemainingQuantity;
        }

        public byte[] ExportProductsToExcel()
        {
            using (var excelFile = new ExcelPackage())
            {
                CreateInfo(excelFile);

                var wsItems = excelFile.Workbook.Worksheets.Add("Items");

                AddHeader(wsItems);

                var productVariants = _productVariantService.GetAll().Where(variant => variant.Product != null).OrderBy(x => x.Product.Id).ToList();

                for (var i = 0; i < productVariants.Count; i++)
                {
                    AddVariant(i, wsItems, productVariants);
                }
                AutofitColumns(wsItems);

                return excelFile.GetAsByteArray();
            }
        }

        private static void AutofitColumns(ExcelWorksheet wsItems)
        {
            wsItems.Cells["A:B"].AutoFitColumns();
            wsItems.Cells["D:D"].AutoFitColumns();
            wsItems.Cells["F:F"].AutoFitColumns();
            wsItems.Cells["I:AH"].AutoFitColumns();
        }

        private void AddVariant(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants)
        {
            var rowId = i + 2;
            AddCoreInfo(i, wsItems, productVariants, rowId);
            AddBrand(i, wsItems, productVariants, rowId);
            AddCategories(i, wsItems, productVariants, rowId);
            AddSpecifications(i, wsItems, productVariants, rowId);
            AddVariantInfo(i, wsItems, productVariants, rowId, _getStockRemainingQuantity);

            AddOptions(i, wsItems, productVariants, rowId);

            AddPriceBreaks(i, wsItems, productVariants, rowId);

            AddUrls(i, wsItems, productVariants, rowId);

            AddPublished(i, wsItems, productVariants, rowId);

            //Images
            AddImages(i, wsItems, productVariants, rowId);
        }

        private void AddImages(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            if (!productVariants[i].Product.Images.Any()) return;

            wsItems.Cells["AA" + rowId].Value = GenerateImageUrlForExport(productVariants[i].Product.Images.First().FileUrl);
            wsItems.Cells["AA" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            if (productVariants[i].Product.Images.Count() > 1)
            {
                wsItems.Cells["AB" + rowId].Value =
                    GenerateImageUrlForExport(productVariants[i].Product.Images.ToList()[1].FileUrl);
                wsItems.Cells["AB" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }
            if (productVariants[i].Product.Images.Count() > 2)
            {
                wsItems.Cells["AC" + rowId].Value =
                    GenerateImageUrlForExport(productVariants[i].Product.Images.ToList()[2].FileUrl);
                wsItems.Cells["AC" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }
        }

        private static void AddPublished(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            if (productVariants[i].Product.Published)
                wsItems.Cells["AF" + rowId].Value = productVariants[i].Product.PublishOn;
            wsItems.Cells["AF" + rowId].Style.Numberformat.Format = "YYYY-MM-DD hh:mm:ss";
        }

        private static void AddUrls(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            if (productVariants[i].Product.Urls.Count > 0)
            {
                foreach (var item in productVariants[i].Product.Urls)
                {
                    wsItems.Cells["AE" + rowId].Value += item.UrlSegment + ",";
                }
                wsItems.Cells["AE" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }
        }

        private static void AddPriceBreaks(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            if (productVariants[i].PriceBreaks.Count > 0)
            {
                foreach (var item in productVariants[i].PriceBreaks)
                {
                    wsItems.Cells["AD" + rowId].Value += item.Quantity + ":" + item.Price.ToString("#.##") + ";";
                }
                wsItems.Cells["AD" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }
        }

        private static void AddOptions(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            for (var v = 0;
                 v <
                 productVariants[i].OptionValues.Count();
                 v++)
            {
                if (v == 0)
                {
                    wsItems.Cells["U" + rowId].Value =
                        productVariants[i].OptionValues[v].ProductOption.Name;
                    wsItems.Cells["V" + rowId].Value = productVariants[i].OptionValues[v].Value;
                }
                if (v == 1)
                {
                    wsItems.Cells["W" + rowId].Value =
                        productVariants[i].OptionValues[v].ProductOption.Name;
                    wsItems.Cells["X" + rowId].Value = productVariants[i].OptionValues[v].Value;
                }
                if (v == 2)
                {
                    wsItems.Cells["Y" + rowId].Value =
                        productVariants[i].OptionValues[v].ProductOption.Name;
                    wsItems.Cells["Z" + rowId].Value = productVariants[i].OptionValues[v].Value;
                }
            }
        }

        private static void AddVariantInfo(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId, IGetStockRemainingQuantity getStockRemainingQuantity)
        {
            wsItems.Cells["K" + rowId].Value = productVariants[i].Name ?? String.Empty;
            wsItems.Cells["L" + rowId].Value = productVariants[i].BasePrice;
            wsItems.Cells["M" + rowId].Value = productVariants[i].PreviousPrice;
            if (productVariants[i].TaxRate != null)
                wsItems.Cells["N" + rowId].Value = productVariants[i].TaxRate.Id;
            wsItems.Cells["O" + rowId].Value = productVariants[i].Weight;
            wsItems.Cells["P" + rowId].Value = getStockRemainingQuantity.Get(productVariants[i]);
            wsItems.Cells["Q" + rowId].Value = productVariants[i].TrackingPolicy;
            wsItems.Cells["R" + rowId].Value = productVariants[i].SKU;
            wsItems.Cells["S" + rowId].Value = productVariants[i].Barcode;
            wsItems.Cells["T" + rowId].Value = productVariants[i].ManufacturerPartNumber;
            wsItems.Cells["AG" + rowId].Value = (productVariants[i].ETag != null) ? productVariants[i].ETag.Name : String.Empty;
        }

        private static void AddSpecifications(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            if (productVariants[i].Product.SpecificationValues.Count > 0)
            {
                foreach (var item in productVariants[i].Product.SpecificationValues)
                {
                    wsItems.Cells["J" + rowId].Value += item.SpecificationName + ":" + item.Value + ";";
                }
                wsItems.Cells["J" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }
        }

        private static void AddCategories(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            if (productVariants[i].Product.Categories.Count > 0)
            {
                foreach (var item in productVariants[i].Product.Categories)
                {
                    wsItems.Cells["I" + rowId].Value += item.UrlSegment + ";";
                }
            }
        }

        private static void AddBrand(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
            if (productVariants[i].Product.BrandPage != null)
                wsItems.Cells["H" + rowId].Value = productVariants[i].Product.BrandPage.Name;
        }

        private static void AddCoreInfo(int i, ExcelWorksheet wsItems, IList<ProductVariant> productVariants, int rowId)
        {
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
            wsItems.Cells["G" + rowId].Value = productVariants[i].Product.ProductAbstract;
            wsItems.Cells["G" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
            wsItems.Cells["AH" + rowId].Value = productVariants[i].Product.SearchResultAbstract;
            wsItems.Cells["AH" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
        }

        private static void AddHeader(ExcelWorksheet wsItems)
        {
            wsItems.Cells["A1:AH1"].Style.Font.Bold = true;
            wsItems.Cells["A:AH"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
            wsItems.Cells["O1"].Value = "Weight (kg)";
            wsItems.Cells["P1"].Value = "Stock";
            wsItems.Cells["Q1"].Value = "Tracking Policy";
            wsItems.Cells["R1"].Value = "SKU";
            wsItems.Cells["S1"].Value = "Barcode";
            wsItems.Cells["T1"].Value = "Manufacturer Part Number";
            wsItems.Cells["U1"].Value = "Option 1 Name";
            wsItems.Cells["V1"].Value = "Option 1 Value";
            wsItems.Cells["W1"].Value = "Option 2 Name";
            wsItems.Cells["X1"].Value = "Option 2 Value";
            wsItems.Cells["Y1"].Value = "Option 3 Name";
            wsItems.Cells["Z1"].Value = "Option 3 Value";
            wsItems.Cells["AA1"].Value = "Image 1";
            wsItems.Cells["AB1"].Value = "Image 2";
            wsItems.Cells["AC1"].Value = "Image 3";
            wsItems.Cells["AD1"].Value = "Price Breaks";
            wsItems.Cells["AE1"].Value = "Url History";
            wsItems.Cells["AF1"].Value = "Publish Date";
            wsItems.Cells["AG1"].Value = "E-Tag";
            wsItems.Cells["AH1"].Value = "Search Result Abstract";
        }

        private static void CreateInfo(ExcelPackage excelFile)
        {
            var wsInfo = excelFile.Workbook.Worksheets.Add("Info");

            wsInfo.Cells["A1:D1"].Style.Font.Bold = true;
            wsInfo.Cells["A:D"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsInfo.Cells["A1"].Value = "MrCMS Version";
            wsInfo.Cells["B1"].Value = "Entity Type for Export";
            wsInfo.Cells["C1"].Value = "Export Date";
            wsInfo.Cells["D1"].Value = "Export Source";

            wsInfo.Cells["A2"].Value = MrCMSHtmlHelperExtensions.AssemblyVersion(null);
            wsInfo.Cells["B2"].Value = "Product";
            wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DD hh:mm:ss";
            wsInfo.Cells["C2"].Value = DateTime.UtcNow;
            wsInfo.Cells["D2"].Value = "MrCMS " + MrCMSHtmlHelperExtensions.AssemblyVersion(null);

            wsInfo.Cells["A:D"].AutoFitColumns();
            wsInfo.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            wsInfo.Cells["A4"].Value = "Please do not change any values inside this file.";
        }

        private string GenerateImageUrlForExport(string imageUrl)
        {
            var siteUrl = "http://" + CurrentRequestData.CurrentSite.BaseUrl;

            return (!imageUrl.Contains("http") && !imageUrl.Contains("https"))
                                   ? (siteUrl + imageUrl + "?update=no")
                                   : imageUrl + "?update=no";
        }
    }
}