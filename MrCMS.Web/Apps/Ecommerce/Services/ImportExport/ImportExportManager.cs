using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using NHibernate;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
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
                                    string url = String.Empty, sku = String.Empty, name = String.Empty;
                                    var productsWorksheet = excelFile.Workbook.Worksheets[2];
                                    //Url
                                    if (productsWorksheet.HasValue(row, 1))
                                        url = productsWorksheet.GetValue<string>(row, 1);
                                    //SKU
                                    if (productsWorksheet.Cells[row, 12] != null && productsWorksheet.Cells[row, 12].Value != null)
                                        sku = productsWorksheet.Cells[row, 12].Value.ToString();
                                    //Name
                                    if (productsWorksheet.Cells[row, 2] != null && productsWorksheet.Cells[row, 2].Value != null)
                                        name = productsWorksheet.Cells[row, 2].Value.ToString();

                                    product = _productService.GetByUrl(url)
                                              ?? new Product
                                                     {
                                                         SKU = sku,
                                                         Name = name,
                                                         Parent = _documentService.GetUniquePage<ProductSearch>()
                                                     };

                                    if (!String.IsNullOrWhiteSpace(product.UrlSegment) && !String.IsNullOrWhiteSpace(sku) && String.IsNullOrWhiteSpace(name))
                                    {
                                        ProductVariant productVariant = _productVariantService.GetProductVariantBySKU(sku);
                                        if (productVariant == null)
                                        {
                                            productVariant = new ProductVariant();
                                        }
                                        productVariant.SKU = sku;

                                        //Base Price
                                        if (productsWorksheet.Cells[row, 9] != null && productsWorksheet.Cells[row, 9].Value != null)
                                        {
                                            decimal BasePrice = 0;
                                            Decimal.TryParse(productsWorksheet.Cells[row, 9].Value.ToString(), out BasePrice);
                                            productVariant.BasePrice = BasePrice;
                                        }
                                        //Previous Price
                                        if (productsWorksheet.Cells[row, 10] != null && productsWorksheet.Cells[row, 10].Value != null)
                                        {
                                            decimal PreviousPrice = 0;
                                            Decimal.TryParse(productsWorksheet.Cells[row, 10].Value.ToString(), out PreviousPrice);
                                            productVariant.PreviousPrice = PreviousPrice;
                                        }
                                        //Stock
                                        if (productsWorksheet.Cells[row, 11] != null && productsWorksheet.Cells[row, 11].Value != null)
                                        {
                                            int StockRemaining = 0;
                                            Int32.TryParse(productsWorksheet.Cells[row, 11].Value.ToString(), out StockRemaining);
                                            productVariant.StockRemaining = StockRemaining;
                                        }
                                        //Tax Rate
                                        if (productsWorksheet.Cells[row, 14] != null && productsWorksheet.Cells[row, 14].Value != null)
                                        {
                                            int taxRateID = 0;
                                            Int32.TryParse(productsWorksheet.Cells[row, 14].Value.ToString(), out taxRateID);
                                            productVariant.TaxRate = _taxRateManager.Get(taxRateID);
                                        }
                                        //Weight
                                        if (productsWorksheet.Cells[row, 15] != null && productsWorksheet.Cells[row, 15].Value != null)
                                        {
                                            decimal Weight = 0;
                                            Decimal.TryParse(productsWorksheet.Cells[row, 15].Value.ToString(), out Weight);
                                            productVariant.Weight = Weight;
                                        }

                                        if (lastAddedProductID != 0)
                                        {
                                            product = _productService.Get(lastAddedProductID);
                                            productVariant.Product = product;
                                        }
                                        else
                                        {
                                            if (productsWorksheet.Cells[row, 1] != null && productsWorksheet.Cells[row, 1].Value != null)
                                            {
                                                product = _productService.GetByUrl(productsWorksheet.Cells[row, 1].Value.ToString());
                                                productVariant.Product = product;
                                            }
                                        }
                                        if (productVariant.Product != null)
                                        {
                                            _productVariantService.Add(productVariant);
                                            productVariant = _productVariantService.GetProductVariantBySKU(productVariant.SKU);

                                            //Option Values
                                            if (productsWorksheet.Cells[row, 13] != null && productsWorksheet.Cells[row, 13].Value != null)
                                            {
                                                string rawOptions = productsWorksheet.Cells[row, 13].Value.ToString();
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
                                                        if (productVariant.Product.AttributeOptions.All(x => x.Id != option.Id))
                                                            product.AttributeOptions.Add(option);
                                                        if (productVariant.AttributeValues.All(x => x.ProductAttributeOption.Id != option.Id))
                                                        {
                                                            productVariant.AttributeValues.Add(new ProductAttributeValue()
                                                            {
                                                                ProductAttributeOption = option,
                                                                ProductVariant = productVariant,
                                                                Value = optionValue[1]
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            _documentService.SaveDocument(product);
                                            _productVariantService.Update(productVariant);
                                        }
                                        else
                                            messages.Add("Product Variant with the following information (SKU:" + productVariant.SKU + ") cannot be stored in database because Parent Product was not identified.");

                                    }
                                    else
                                    {
                                        //Description
                                        if (productsWorksheet.Cells[row, 3] != null && productsWorksheet.Cells[row, 3].Value != null)
                                            product.BodyContent = productsWorksheet.Cells[row, 3].Value.ToString();
                                        //Meta Title
                                        if (productsWorksheet.Cells[row, 4] != null && productsWorksheet.Cells[row, 4].Value != null)
                                            product.MetaTitle = productsWorksheet.Cells[row, 4].Value.ToString();
                                        //Meta Description
                                        if (productsWorksheet.Cells[row, 5] != null && productsWorksheet.Cells[row, 5].Value != null)
                                            product.MetaDescription = productsWorksheet.Cells[row, 5].Value.ToString();
                                        //Meta Keywords
                                        if (productsWorksheet.Cells[row, 6] != null && productsWorksheet.Cells[row, 6].Value != null)
                                            product.MetaKeywords = productsWorksheet.Cells[row, 6].Value.ToString();
                                        //Abstract
                                        if (productsWorksheet.Cells[row, 7] != null && productsWorksheet.Cells[row, 7].Value != null)
                                            product.Abstract = productsWorksheet.Cells[row, 7].Value.ToString();
                                        //Brand
                                        if (productsWorksheet.Cells[row, 8] != null && productsWorksheet.Cells[row, 8].Value != null)
                                        {
                                            string brandName = productsWorksheet.Cells[row, 8].Value.ToString();
                                            if (!_brandService.AnyExistingBrandsWithName(brandName, 0))
                                                _brandService.Add(new Brand() { Name = brandName });
                                            product.Brand = _brandService.GetBrandByName(brandName); ;
                                        }
                                        //Base Price
                                        if (productsWorksheet.Cells[row, 9] != null && productsWorksheet.Cells[row, 9].Value != null)
                                        {
                                            decimal BasePrice = 0;
                                            Decimal.TryParse(productsWorksheet.Cells[row, 9].Value.ToString(), out BasePrice);
                                            product.BasePrice = BasePrice;
                                        }
                                        //Previous Price
                                        if (productsWorksheet.Cells[row, 10] != null && productsWorksheet.Cells[row, 10].Value != null)
                                        {
                                            decimal PreviousPrice = 0;
                                            Decimal.TryParse(productsWorksheet.Cells[row, 10].Value.ToString(), out PreviousPrice);
                                            product.PreviousPrice = PreviousPrice;
                                        }
                                        //Stock
                                        if (productsWorksheet.Cells[row, 11] != null && productsWorksheet.Cells[row, 11].Value != null)
                                        {
                                            int StockRemaining = 0;
                                            Int32.TryParse(productsWorksheet.Cells[row, 11].Value.ToString(), out StockRemaining);
                                            product.StockRemaining = StockRemaining;
                                        }
                                        //Tax Rate
                                        if (productsWorksheet.Cells[row, 14] != null && productsWorksheet.Cells[row, 14].Value != null)
                                        {
                                            int taxRateID = 0;
                                            Int32.TryParse(productsWorksheet.Cells[row, 14].Value.ToString(), out taxRateID);
                                            product.TaxRate = _taxRateManager.Get(taxRateID);
                                        }
                                        //Weight
                                        if (productsWorksheet.Cells[row, 15] != null && productsWorksheet.Cells[row, 15].Value != null)
                                        {
                                            decimal Weight = 0;
                                            Decimal.TryParse(productsWorksheet.Cells[row, 15].Value.ToString(), out Weight);
                                            product.Weight = Weight;
                                        }
                                        //Categories
                                        if (productsWorksheet.Cells[row, 16] != null && productsWorksheet.Cells[row, 16].Value != null)
                                        {
                                            string rawCategories = productsWorksheet.Cells[row, 16].Value.ToString();
                                            string[] categories = rawCategories.Split(';');
                                            foreach (var item in categories)
                                            {
                                                int categoryID = 0;
                                                Int32.TryParse(item, out categoryID);
                                                Category category = _documentService.GetDocument<Category>(categoryID);
                                                if (category != null && !product.Categories.Any(x => x.Id == category.Id))
                                                    product.Categories.Add(category);
                                            }
                                        }
                                        if (String.IsNullOrWhiteSpace(product.UrlSegment))
                                        {
                                            product.UrlSegment = url;
                                            _documentService.AddDocument<Product>(product);
                                        }
                                        else
                                        {
                                            _documentService.SaveDocument<Product>(product);
                                        }
                                        product = _productService.Get(product.Id);

                                        //Specifications
                                        if (productsWorksheet.Cells[row, 17] != null && productsWorksheet.Cells[row, 17].Value != null)
                                        {
                                            string rawSpecifications = productsWorksheet.Cells[row, 17].Value.ToString();
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
                                                    if (!product.SpecificationValues.Any(x => x.ProductSpecificationAttribute.Id == option.Id && x.Product.Id == product.Id))
                                                        product.SpecificationValues.Add(new ProductSpecificationValue() { ProductSpecificationAttribute = option, Value = specificationValue[1], Product = product });
                                                }
                                            }
                                        }
                                        lastAddedProductID = product.Id;
                                        _documentService.SaveDocument(product);
                                    }
                                }
                                messages.Add("All products were successfully imported.");
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
            catch (Exception)
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
                wsInfo.Cells["B1"].Value = "Entity Type for Export";
                wsInfo.Cells["C1"].Value = "Export Date";

                wsInfo.Cells["A2"].Value = MrCMSHtmlHelper.AssemblyVersion(null);
                wsInfo.Cells["B2"].Value = "Product";
                wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";
                wsInfo.Cells["C2"].Value = DateTime.UtcNow;
                wsInfo.Cells["A:C"].AutoFitColumns();
                wsInfo.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                wsInfo.Cells["A4"].Value = "Please do not change any values inside this file.";

                ExcelWorksheet wsProducts = excelFile.Workbook.Worksheets.Add("Products");

                wsProducts.Cells["A1:Q1"].Style.Font.Bold = true;
                wsProducts.Cells["A:Q"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsProducts.Cells["A1"].Value = "Url (Must not be changed!)";
                wsProducts.Cells["B1"].Value = "Name";
                wsProducts.Cells["C1"].Value = "Description";
                wsProducts.Cells["D1"].Value = "SEO Title";
                wsProducts.Cells["E1"].Value = "SEO Description";
                wsProducts.Cells["F1"].Value = "SEO Title";
                wsProducts.Cells["G1"].Value = "Abstract";
                wsProducts.Cells["H1"].Value = "Brand";
                wsProducts.Cells["I1"].Value = "Price";
                wsProducts.Cells["J1"].Value = "Previous Price";
                wsProducts.Cells["K1"].Value = "Stock";
                wsProducts.Cells["L1"].Value = "SKU";
                wsProducts.Cells["M1"].Value = "Option Values";
                wsProducts.Cells["N1"].Value = "Tax Rate";
                wsProducts.Cells["O1"].Value = "Weight";
                wsProducts.Cells["P1"].Value = "Categories";
                wsProducts.Cells["Q1"].Value = "Specifications";

                IList<Product> products = _productService.GetAll();

                int variantCounter = 0;
                int rowNumber = 0;
                for (int i = 0; i < products.Count; i++)
                {
                    rowNumber = i + 2 + variantCounter;
                    wsProducts.Cells["A" + rowNumber.ToString()].Value = products[i].UrlSegment;
                    wsProducts.Cells["B" + rowNumber.ToString()].Value = products[i].Name;
                    wsProducts.Cells["C" + rowNumber.ToString()].Value = products[i].BodyContent;
                    wsProducts.Cells["D" + rowNumber.ToString()].Value = products[i].MetaTitle;
                    wsProducts.Cells["E" + rowNumber.ToString()].Value = products[i].MetaDescription;
                    wsProducts.Cells["F" + rowNumber.ToString()].Value = products[i].MetaKeywords;
                    wsProducts.Cells["G" + rowNumber.ToString()].Value = products[i].Abstract;
                    if (products[i].Brand != null)
                        wsProducts.Cells["H" + rowNumber.ToString()].Value = products[i].Brand.Name;
                    wsProducts.Cells["I" + rowNumber.ToString()].Value = products[i].BasePrice;
                    wsProducts.Cells["J" + rowNumber.ToString()].Value = products[i].PreviousPrice;
                    wsProducts.Cells["K" + rowNumber.ToString()].Value = products[i].StockRemaining;
                    wsProducts.Cells["L" + rowNumber.ToString()].Value = products[i].SKU;
                    if (products[i].TaxRate != null)
                        wsProducts.Cells["N" + rowNumber.ToString()].Value = products[i].TaxRate.Id;
                    wsProducts.Cells["O" + rowNumber.ToString()].Value = products[i].Weight;
                    if (products[i].Categories.Count > 0)
                    {
                        foreach (var item in products[i].Categories)
                        {
                            wsProducts.Cells["P" + rowNumber.ToString()].Value += item.Id + ";";
                        }
                    }
                    if (products[i].SpecificationValues.Count > 0)
                    {
                        foreach (var item in products[i].SpecificationValues)
                        {
                            wsProducts.Cells["Q" + rowNumber.ToString()].Value += item.ProductSpecificationAttribute.Name + ":" + item.Value + ";";
                        }
                    }

                    variantCounter = products[i].Variants.Count;

                    for (int j = 0; j < products[i].Variants.Count; j++)
                    {
                        wsProducts.Cells["A" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].Product.UrlSegment;
                        wsProducts.Cells["I" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].BasePrice;
                        wsProducts.Cells["J" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].PreviousPrice;
                        wsProducts.Cells["K" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].StockRemaining;
                        wsProducts.Cells["L" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].SKU;
                        if (products[i].TaxRate != null)
                            wsProducts.Cells["N" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].TaxRate.Id;
                        wsProducts.Cells["O" + (j + 1 + rowNumber).ToString()].Value = products[i].Variants[j].Weight;
                        foreach (var item in products[i].Variants[j].AttributeValues)
                        {
                            wsProducts.Cells["M" + (j + 1 + rowNumber).ToString()].Value += item.ProductAttributeOption.Name + ":" + item.Value + ";";
                        }
                    }

                }

                wsProducts.Cells["A:B"].AutoFitColumns();
                wsProducts.Cells["D:F"].AutoFitColumns();
                wsProducts.Cells["I:Q"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }
    }
}