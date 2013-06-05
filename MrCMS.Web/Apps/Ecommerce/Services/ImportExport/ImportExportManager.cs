
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly ISession _session;
        private readonly IProductService _productService;

        public ImportExportManager(ISession session, IProductService productService)
        {
            _session = session;
            _productService = productService;
        }

        public byte[] ExportProductsToExcel()
        {
            using (ExcelPackage excelFile = new ExcelPackage())
            {
                ExcelWorksheet wsInfo = excelFile.Workbook.Worksheets.Add("Info");
                
                wsInfo.Cells["A1:C1"].Style.Font.Bold = true;
                wsInfo.Cells["A:C"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsInfo.Cells["A1"].Value = "MrCMS Version";
                wsInfo.Cells["B1"].Value = "Export Entity";
                wsInfo.Cells["C1"].Value = "Export Date";

                wsInfo.Cells["A2"].Value = MrCMSHtmlHelper.AssemblyVersion(null);
                wsInfo.Cells["B2"].Value = "Product";
                wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";
                wsInfo.Cells["C2"].Value = DateTime.UtcNow;
                wsInfo.Cells["A:C"].AutoFitColumns();

                ExcelWorksheet wsProducts = excelFile.Workbook.Worksheets.Add("Products");

                wsProducts.Cells["A1:R1"].Style.Font.Bold = true;
                wsProducts.Cells["A:R"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsProducts.Cells["A1"].Value = "Name";
                wsProducts.Cells["B1"].Value = "Description";
                wsProducts.Cells["C1"].Value = "SEO Title";
                wsProducts.Cells["D1"].Value = "SEO Description";
                wsProducts.Cells["E1"].Value = "SEO Keywords";
                wsProducts.Cells["F1"].Value = "Abstract";
                wsProducts.Cells["G1"].Value = "Brand";
                wsProducts.Cells["H1"].Value = "Base Price";
                wsProducts.Cells["I1"].Value = "Previous Price";
                wsProducts.Cells["J1"].Value = "Stock";
                wsProducts.Cells["K1"].Value = "SKU";
                wsProducts.Cells["L1"].Value = "Tax Rate";
                wsProducts.Cells["M1"].Value = "Weight";
                wsProducts.Cells["N1"].Value = "Categories";
                wsProducts.Cells["O1"].Value = "Specifications";
                wsProducts.Cells["P1"].Value = "Options";
                wsProducts.Cells["Q1"].Value = "Option Values";
                wsProducts.Cells["R1"].Value = "Parent SKU";

                IList<Product> products = _productService.GetAll();

                int variantCounter = 0;
                int rowNumber = 0;
                for (int i = 0; i < products.Count; i++)
                {
                    rowNumber = i + 2 + variantCounter;
                    wsProducts.Cells["A" + rowNumber.ToString()].Value = products[i].Name;
                    wsProducts.Cells["B" + rowNumber.ToString()].Value = products[i].BodyContent;
                    wsProducts.Cells["C" + rowNumber.ToString()].Value = products[i].MetaTitle;
                    wsProducts.Cells["D" + rowNumber.ToString()].Value = products[i].MetaDescription;
                    wsProducts.Cells["E" + rowNumber.ToString()].Value = products[i].MetaKeywords;
                    wsProducts.Cells["F" + rowNumber.ToString()].Value = products[i].Abstract;
                    if (products[i].Brand != null)
                        wsProducts.Cells["G" + rowNumber.ToString()].Value = products[i].Brand.Name;
                    wsProducts.Cells["H" + rowNumber.ToString()].Value = products[i].BasePrice;
                    wsProducts.Cells["I" + rowNumber.ToString()].Value = products[i].PreviousPrice;
                    wsProducts.Cells["J" + rowNumber.ToString()].Value = products[i].StockRemaining;
                    wsProducts.Cells["K" + rowNumber.ToString()].Value = products[i].SKU;
                    if (products[i].TaxRate != null)
                        wsProducts.Cells["L" + rowNumber.ToString()].Value = products[i].TaxRate.Id;
                    wsProducts.Cells["M" + rowNumber.ToString()].Value = products[i].Weight;
                    if (products[i].Categories.Count > 0)
                    {
                        foreach (var item in products[i].Categories)
	                    {
                            wsProducts.Cells["N" + rowNumber.ToString()].Value += item.Id + ";";
	                    }
                    }
                    if (products[i].SpecificationValues.Count > 0)
                    {
                        foreach (var item in products[i].SpecificationValues)
                        {
                            wsProducts.Cells["O" + rowNumber.ToString()].Value += item.Option.Name + ":" + item.Value + ";";
                        }
                    }
                    if (products[i].AttributeOptions.Count > 0)
                    {
                        foreach (var item in products[i].AttributeOptions)
                        {
                            wsProducts.Cells["P" + rowNumber.ToString()].Value += item.Name + ";";
                        }
                    }

                    variantCounter = products[i].Variants.Count;

                    for (int j = 0; j < products[i].Variants.Count; j++)
                    {
                        wsProducts.Cells["H" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].BasePrice;
                        wsProducts.Cells["I" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].PreviousPrice;
                        wsProducts.Cells["J" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].StockRemaining;
                        wsProducts.Cells["K" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].SKU;
                        if (products[i].TaxRate != null)
                            wsProducts.Cells["L" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].TaxRate.Id;
                        wsProducts.Cells["M" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].Weight;
                        foreach (var item in products[i].Variants[j].AttributeValues)
                        {
                            wsProducts.Cells["Q" + (j + 1 + rowNumber).ToString()].Value += item.ProductAttributeOption.Name + ":" + item.Value + ";";
                        }
                        wsProducts.Cells["R" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].Product.SKU;
                    }

                }

                wsProducts.Cells["A:R"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }
    }
}