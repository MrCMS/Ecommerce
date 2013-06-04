
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
                wsInfo.Cells["B1"].Value = "Export Entity Type";
                wsInfo.Cells["C1"].Value = "Export Date";

                wsInfo.Cells["A2"].Value = MrCMSHtmlHelper.AssemblyVersion(null);
                wsInfo.Cells["B2"].Value = "Product";
                wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";
                wsInfo.Cells["C2"].Value = DateTime.UtcNow;
                wsInfo.Cells["A:C"].AutoFitColumns();

                ExcelWorksheet wsProducts = excelFile.Workbook.Worksheets.Add("Products");

                wsProducts.Cells["A1:O1"].Style.Font.Bold = true;
                wsProducts.Cells["A:O"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsProducts.Cells["A1"].Value = "Name";
                wsProducts.Cells["B1"].Value = "Description";
                wsProducts.Cells["C1"].Value = "SEO Title";
                wsProducts.Cells["D1"].Value = "SEO Description";
                wsProducts.Cells["E1"].Value = "SEO Keywords";
                wsProducts.Cells["F1"].Value = "Abstract";
                wsProducts.Cells["G1"].Value = "Brand";
                wsProducts.Cells["H1"].Value = "Price";
                wsProducts.Cells["I1"].Value = "Previous Price";
                wsProducts.Cells["J1"].Value = "Stock";
                wsProducts.Cells["K1"].Value = "SKU";
                wsProducts.Cells["L1"].Value = "Tax Rate";
                wsProducts.Cells["M1"].Value = "Weight";
                wsProducts.Cells["N1"].Value = "Categories";
                wsProducts.Cells["O1"].Value = "Specifications";

                IList<Product> products = _productService.GetAll();

                for (int i = 0; i < products.Count; i++)
                {
                    wsProducts.Cells["A" + (i + 2).ToString()].Value = products[i].Name;
                    wsProducts.Cells["B" + (i + 2).ToString()].Value = products[i].BodyContent;
                    wsProducts.Cells["C" + (i + 2).ToString()].Value = products[i].MetaTitle;
                    wsProducts.Cells["D" + (i + 2).ToString()].Value = products[i].MetaDescription;
                    wsProducts.Cells["E" + (i + 2).ToString()].Value = products[i].MetaKeywords;
                    wsProducts.Cells["F" + (i + 2).ToString()].Value = products[i].Abstract;
                    if (products[i].Brand != null)
                        wsProducts.Cells["G" + (i + 2).ToString()].Value = products[i].Brand.Name;
                    wsProducts.Cells["H" + (i + 2).ToString()].Value = products[i].Price;
                    wsProducts.Cells["I" + (i + 2).ToString()].Value = products[i].PreviousPrice;
                    wsProducts.Cells["J" + (i + 2).ToString()].Value = products[i].StockRemaining;
                    wsProducts.Cells["K" + (i + 2).ToString()].Value = products[i].SKU;
                    if (products[i].TaxRate != null)
                        wsProducts.Cells["L" + (i + 2).ToString()].Value = products[i].TaxRate.Id;
                    wsProducts.Cells["M" + (i + 2).ToString()].Value = products[i].Weight;
                    if (products[i].Categories.Count > 0)
                    {
                        foreach (var item in products[i].Categories)
	                    {
                            wsProducts.Cells["N" + (i + 2).ToString()].Value += item.Id + ";";
	                    }
                    }
                    if (products[i].SpecificationValues.Count > 0)
                    {
                        foreach (var item in products[i].SpecificationValues)
                        {
                            wsProducts.Cells["O" + (i + 2).ToString()].Value += item.Option.Name + ":" + item.Value + ";";
                        }
                    }
                }
                wsProducts.Cells["A:O"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }
    }
}