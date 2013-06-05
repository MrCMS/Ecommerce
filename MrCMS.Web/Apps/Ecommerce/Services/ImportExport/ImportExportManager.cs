
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using NHibernate;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly ISession _session;
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IDocumentService _documentService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IBrandService _brandService;
        private readonly ITaxRateManager _taxRateManager;

        public ImportExportManager(ISession session, IProductService productService, IProductVariantService productVariantService,
            IDocumentService documentService, IProductOptionManager productOptionManager, IBrandService brandService, ITaxRateManager taxRateManager)
        {
            _session = session;
            _productService = productService;
            _productVariantService = productVariantService;
            _documentService = documentService;
            _productOptionManager = productOptionManager;
            _brandService = brandService;
            _taxRateManager = taxRateManager;
        }

        public List<string> ImportProductsFromExcel(HttpPostedFileBase file)
        {
            List<string> messages = new List<string>();
            try
            {
                using (ExcelPackage excelFile = new ExcelPackage(file.InputStream))
                {
                    if (excelFile.Workbook != null)
                    {
                        if (excelFile.Workbook.Worksheets.Count > 0)
                        {
                            if (excelFile.Workbook.Worksheets[1].Name == "Info" && excelFile.Workbook.Worksheets[2].Name == "Products")
                            {
                                var rowCount = excelFile.Workbook.Worksheets[2].Dimension.End.Row;
                                int lastAddedProductID = 0;

                                for (var row = 2; row <= rowCount; row++)
                                {
                                    Product product = new Product();

                                    //Name
                                    if (excelFile.Workbook.Worksheets[2].Cells[row, 1] != null && excelFile.Workbook.Worksheets[2].Cells[row, 1].Value != null)
                                        product.Name = excelFile.Workbook.Worksheets[2].Cells[row, 1].Value.ToString();
                                    //SKU
                                    if (excelFile.Workbook.Worksheets[2].Cells[row, 11] != null && excelFile.Workbook.Worksheets[2].Cells[row, 11].Value != null)
                                        product.SKU = excelFile.Workbook.Worksheets[2].Cells[row, 11].Value.ToString();

                                    if (String.IsNullOrWhiteSpace(product.Name) && !String.IsNullOrWhiteSpace(product.SKU))
                                    {
                                        if (!_productVariantService.AnyExistingProductVariantWithSKU(product.SKU, 0))
                                        {
                                            ProductVariant productVariant = new ProductVariant();
                                            productVariant.SKU = product.SKU;
                                            //Base Price
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 8] != null && excelFile.Workbook.Worksheets[2].Cells[row, 8].Value != null)
                                            {
                                                decimal BasePrice = 0;
                                                Decimal.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 8].Value.ToString(), out BasePrice);
                                                productVariant.BasePrice = BasePrice;
                                            }
                                            //Previous Price
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 9] != null && excelFile.Workbook.Worksheets[2].Cells[row, 9].Value != null)
                                            {
                                                decimal PreviousPrice = 0;
                                                Decimal.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 9].Value.ToString(), out PreviousPrice);
                                                productVariant.PreviousPrice = PreviousPrice;
                                            }
                                            //Stock
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 10] != null && excelFile.Workbook.Worksheets[2].Cells[row, 10].Value != null)
                                            {
                                                int StockRemaining = 0;
                                                Int32.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 10].Value.ToString(), out StockRemaining);
                                                productVariant.StockRemaining = StockRemaining;
                                            }
                                            //Tax Rate
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 12] != null && excelFile.Workbook.Worksheets[2].Cells[row, 12].Value != null)
                                            {
                                                int taxRateID = 0;
                                                Int32.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 12].Value.ToString(), out taxRateID);
                                                productVariant.TaxRate = _taxRateManager.Get(taxRateID);
                                            }
                                            //Weight
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 13] != null && excelFile.Workbook.Worksheets[2].Cells[row, 13].Value != null)
                                            {
                                                decimal Weight = 0;
                                                Decimal.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 13].Value.ToString(), out Weight);
                                                productVariant.Weight = Weight;
                                            }


                                            if (lastAddedProductID != 0)
                                                productVariant.Product = _productService.Get(lastAddedProductID);
                                            else
                                            {
                                                if (excelFile.Workbook.Worksheets[2].Cells[row, 18] != null && excelFile.Workbook.Worksheets[2].Cells[row, 18].Value != null)
                                                {
                                                    productVariant.Product = _productService.GetByName(excelFile.Workbook.Worksheets[2].Cells[row, 18].Value.ToString());
                                                }
                                            }
                                            if (productVariant.Product != null)
                                            {
                                                _productVariantService.Add(productVariant);
                                                productVariant = _productVariantService.GetProductVariantBySKU(productVariant.SKU);

                                                //Option Values
                                                if (excelFile.Workbook.Worksheets[2].Cells[row, 17] != null && excelFile.Workbook.Worksheets[2].Cells[row, 17].Value != null)
                                                {
                                                    string rawOptions = excelFile.Workbook.Worksheets[2].Cells[row, 17].Value.ToString();
                                                    string[] options = rawOptions.Split(';');
                                                    foreach (var item in options)
                                                    {
                                                        if (item != String.Empty)
                                                        {
                                                            string[] optionValue = item.Split(':');

                                                            if (!_productOptionManager.AnyExistingAttributeOptionsWithName(optionValue[0]))
                                                            {
                                                                _productOptionManager.AddAttributeOption(new ProductAttributeOption() { Name = optionValue[0] });
                                                            }

                                                            ProductAttributeOption option = _productOptionManager.GetAttributeOptionByName(optionValue[0]);
                                                            productVariant.AttributeValues.Add(new ProductAttributeValue()
                                                            {
                                                                ProductAttributeOption = option,
                                                                ProductVariant = productVariant,
                                                                Value = optionValue[1]
                                                            });
                                                        }
                                                    }
                                                }

                                                _productVariantService.Update(productVariant);
                                            }
                                            else
                                                messages.Add("Product Variant with the following information (SKU:" + productVariant.SKU + " - Name:" + productVariant.Name + ") cannot be stored in database because Parent Product was not identified.");
                                        }
                                        else
                                            messages.Add("Product Variant with the following information (SKU:" + product.SKU + " - Name:" + product.Name + ") is already stored in the database.");
                                    }
                                    else
                                    {
                                        if (!_productService.AnyExistingProductWithSKU(product.SKU, 0))
                                        {
                                            //Description
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 2] != null && excelFile.Workbook.Worksheets[2].Cells[row, 2].Value != null)
                                                product.BodyContent = excelFile.Workbook.Worksheets[2].Cells[row, 2].Value.ToString();
                                            //Meta Title
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 3] != null && excelFile.Workbook.Worksheets[2].Cells[row, 3].Value != null)
                                                product.MetaTitle = excelFile.Workbook.Worksheets[2].Cells[row, 3].Value.ToString();
                                            //Meta Description
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 4] != null && excelFile.Workbook.Worksheets[2].Cells[row, 4].Value != null)
                                                product.MetaDescription = excelFile.Workbook.Worksheets[2].Cells[row, 4].Value.ToString();
                                            //Meta Keywords
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 5] != null && excelFile.Workbook.Worksheets[2].Cells[row, 5].Value != null)
                                                product.MetaKeywords = excelFile.Workbook.Worksheets[2].Cells[row, 5].Value.ToString();
                                            //Abstract
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 6] != null && excelFile.Workbook.Worksheets[2].Cells[row, 6].Value != null)
                                                product.Abstract = excelFile.Workbook.Worksheets[2].Cells[row, 6].Value.ToString();
                                            //Brand
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 7] != null && excelFile.Workbook.Worksheets[2].Cells[row, 7].Value != null)
                                            {
                                                string brandName = excelFile.Workbook.Worksheets[2].Cells[row, 7].Value.ToString();
                                                if (!_brandService.AnyExistingBrandsWithName(brandName, 0))
                                                    _brandService.Add(new Brand() { Name = brandName });
                                                product.Brand = _brandService.GetBrandByName(brandName); ;
                                            }
                                            //Base Price
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 8] != null && excelFile.Workbook.Worksheets[2].Cells[row, 8].Value != null)
                                            {
                                                decimal BasePrice = 0;
                                                Decimal.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 8].Value.ToString(), out BasePrice);
                                                product.BasePrice = BasePrice;
                                            }
                                            //Previous Price
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 9] != null && excelFile.Workbook.Worksheets[2].Cells[row, 9].Value != null)
                                            {
                                                decimal PreviousPrice = 0;
                                                Decimal.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 9].Value.ToString(), out PreviousPrice);
                                                product.PreviousPrice = PreviousPrice;
                                            }
                                            //Stock
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 10] != null && excelFile.Workbook.Worksheets[2].Cells[row, 10].Value != null)
                                            {
                                                int StockRemaining = 0;
                                                Int32.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 10].Value.ToString(), out StockRemaining);
                                                product.StockRemaining = StockRemaining;
                                            }
                                            //Tax Rate
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 12] != null && excelFile.Workbook.Worksheets[2].Cells[row, 12].Value != null)
                                            {
                                                int taxRateID = 0;
                                                Int32.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 12].Value.ToString(), out taxRateID);
                                                product.TaxRate = _taxRateManager.Get(taxRateID);
                                            }
                                            //Weight
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 13] != null && excelFile.Workbook.Worksheets[2].Cells[row, 13].Value != null)
                                            {
                                                decimal Weight = 0;
                                                Decimal.TryParse(excelFile.Workbook.Worksheets[2].Cells[row, 13].Value.ToString(), out Weight);
                                                product.Weight = Weight;
                                            }
                                            //Categories
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 14] != null && excelFile.Workbook.Worksheets[2].Cells[row, 14].Value != null)
                                            {
                                                string rawCategories = excelFile.Workbook.Worksheets[2].Cells[row, 14].Value.ToString();
                                                string[] categories = rawCategories.Split(';');
                                                foreach (var item in categories)
                                                {
                                                    int categoryID = 0;
                                                    Int32.TryParse(item, out categoryID);
                                                    Category category = _documentService.GetDocument<Category>(categoryID);
                                                    if (category != null)
                                                    {
                                                        product.Categories.Add(category);
                                                    }
                                                }
                                            }

                                            product.UrlSegment = _documentService.GetDocumentUrl(product.Name, null, false);
                                            _documentService.AddDocument<Product>(product);
                                            product = _productService.Get(product.Id);

                                            //Specifications
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 15] != null && excelFile.Workbook.Worksheets[2].Cells[row, 15].Value != null)
                                            {
                                                string rawSpecifications = excelFile.Workbook.Worksheets[2].Cells[row, 15].Value.ToString();
                                                string[] specifications = rawSpecifications.Split(';');
                                                foreach (var item in specifications)
                                                {
                                                    if (item != String.Empty)
                                                    {
                                                        string[] specificationValue = item.Split(':');

                                                        if (!_productOptionManager.AnyExistingSpecificationAttributesWithName(specificationValue[0]))
                                                        {
                                                            _productOptionManager.AddSpecificationAttribute(new ProductSpecificationAttribute() { Name = specificationValue[0] });
                                                        }

                                                        ProductSpecificationAttribute option = _productOptionManager.GetSpecificationAttributeByName(specificationValue[0]);
                                                        product.SpecificationValues.Add(new ProductSpecificationValue() { Option = option, Value = specificationValue[1], Product = product });
                                                    }
                                                }
                                            }
                                            //Options
                                            if (excelFile.Workbook.Worksheets[2].Cells[row, 16] != null && excelFile.Workbook.Worksheets[2].Cells[row, 16].Value != null)
                                            {
                                                string rawOptions = excelFile.Workbook.Worksheets[2].Cells[row, 16].Value.ToString();
                                                string[] options = rawOptions.Split(';');
                                                foreach (var item in options)
                                                {
                                                    if (!_productOptionManager.AnyExistingAttributeOptionsWithName(item))
                                                    {
                                                        _productOptionManager.AddAttributeOption(new ProductAttributeOption() { Name = item });
                                                    }

                                                    ProductAttributeOption option = _productOptionManager.GetAttributeOptionByName(item);
                                                    product.AttributeOptions.Add(option);
                                                }
                                            }
                                            lastAddedProductID = product.Id;
                                            _documentService.SaveDocument<Product>(product);
                                        }
                                        else
                                            messages.Add("Product with the following information (SKU:" + product.SKU + " - Name:" + product.Name + ") is already stored in the database.");
                                    }
                                }
                            }
                            else
                                messages.Add("No Info or Products worksheets in file.");
                        }
                        else
                            messages.Add("No worksheets in file.");
                    }
                    else
                        messages.Add("Error reading file.");
                }
            }
            catch (Exception ex)
            {
                messages.Add("Error reading file. It is possible that file is corrupted.");
            }
            return messages;
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
                wsProducts.Cells["R1"].Value = "Parent Name";

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
                        wsProducts.Cells["R" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].Product.Name;
                    }

                }

                wsProducts.Cells["A:R"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }
    }
}