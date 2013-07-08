
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
using System.Linq;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using System.Net;
using System.IO;
using MrCMS.Entities.Documents.Media;
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
        private readonly IFileService _fileService;

        public ImportExportManager(ISession session, IProductService productService, IProductVariantService productVariantService,
            IDocumentService documentService, IProductOptionManager productOptionManager, IBrandService brandService, ITaxRateManager taxRateManager,
            IFileService fileService)
        {
            _session = session;
            _productService = productService;
            _productVariantService = productVariantService;
            _documentService = documentService;
            _productOptionManager = productOptionManager;
            _brandService = brandService;
            _taxRateManager = taxRateManager;
            _fileService = fileService;
        }

        public List<string> ImportProductsFromExcel(HttpPostedFileBase file)
        {
            List<string> messages = new List<string>();
            //try
            //{
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
                                var variantsInImportedFile = new List<ProductVariant>();

                                for (var row = 2; row <= rowCount; row++)
                                {
                                    string url = GetValueFromRow(excelFile, 2, row, 1)
                                    , productName = GetValueFromRow(excelFile, 2, row, 2)
                                    , description = GetValueFromRow(excelFile, 2, row, 3)
                                    , seoTitle = GetValueFromRow(excelFile, 2, row, 4)
                                    , seoDescription = GetValueFromRow(excelFile, 2, row, 5)
                                    , seoKeywords = GetValueFromRow(excelFile, 2, row, 6)
                                    , productAbstract = GetValueFromRow(excelFile, 2, row, 7)
                                    , brand = GetValueFromRow(excelFile, 2, row, 8)
                                    , categories = GetValueFromRow(excelFile, 2, row, 9)
                                    , specifications = GetValueFromRow(excelFile, 2, row, 10)
                                    , variantName = GetValueFromRow(excelFile, 2, row, 11)
                                    , price = GetValueFromRow(excelFile, 2, row, 12)
                                    , previousPrice = GetValueFromRow(excelFile, 2, row, 13)
                                    , taxRate = GetValueFromRow(excelFile, 2, row, 14)
                                    , weight = GetValueFromRow(excelFile, 2, row, 15)
                                    , stock = GetValueFromRow(excelFile, 2, row, 16)
                                    , trackingPolicy = GetValueFromRow(excelFile, 2, row, 17)
                                    , sku = GetValueFromRow(excelFile, 2, row, 18)
                                    , barcode = GetValueFromRow(excelFile, 2, row, 19)
                                    , option1Name = GetValueFromRow(excelFile, 2, row, 20)
                                    , option1Value = GetValueFromRow(excelFile, 2, row, 21)
                                    , option2Name = GetValueFromRow(excelFile, 2, row, 22)
                                    , option2Value = GetValueFromRow(excelFile, 2, row, 23)
                                    , option3Name = GetValueFromRow(excelFile, 2, row, 24)
                                    , option3Value = GetValueFromRow(excelFile, 2, row, 25)
                                    , image1 = GetValueFromRow(excelFile, 2, row, 26)
                                    , image2 = GetValueFromRow(excelFile, 2, row, 27)
                                    , image3 = GetValueFromRow(excelFile, 2, row, 28);

                                    Product product = _productService.GetByUrl(url);

                                    if (product == null)
                                        product = new Product();
                                    else
                                        lastAddedProductID = product.Id;

                                    product.Parent = _documentService.GetUniquePage<ProductSearch>();
                                    product.Name = productName;
                                    product.BodyContent = description;
                                    product.MetaTitle = seoTitle;
                                    product.MetaDescription = seoDescription;
                                    product.MetaKeywords = seoKeywords;
                                    product.Abstract = productAbstract;
                                    product.PublishOn = DateTime.UtcNow;
                                    if (!_brandService.AnyExistingBrandsWithName(brand, 0))
                                        _brandService.Add(new Brand() { Name = brand });
                                    product.Brand = _brandService.GetBrandByName(brand);

                                    //Categories
                                    string[] Cats = categories.Split(';');
                                    foreach (var item in Cats)
                                    {
                                        int categoryID = 0;
                                        Int32.TryParse(item, out categoryID);
                                        Category category = _documentService.GetDocument<Category>(categoryID);
                                        if (category != null && product.Categories.Where(x => x.Id == category.Id).Count() == 0)
                                            product.Categories.Add(category);
                                    }
                                    if(lastAddedProductID==0)
                                    {
                                        product.UrlSegment = url;
                                        _documentService.AddDocument<Product>(product);
                                        //Delete Variant which is saved via DocumentTypeSetup
                                        product.Variants.Clear();
                                    }
                                    else
                                         _documentService.SaveDocument<Product>(product);

                                    product = _productService.Get(product.Id);

                                    if (!product.Images.Any())
                                    {
                                        if (!String.IsNullOrWhiteSpace(image1))
                                            AddFile(image1.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                                        if (!String.IsNullOrWhiteSpace(image2))
                                            AddFile(image2.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                                        if (!String.IsNullOrWhiteSpace(image3))
                                            AddFile(image3.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrWhiteSpace(image1) && image1.Contains("?update=yes"))
                                            AddFile(image1.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                                        if (!String.IsNullOrWhiteSpace(image2) && image2.Contains("?update=yes"))
                                            AddFile(image2.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                                        if (!String.IsNullOrWhiteSpace(image3) && image3.Contains("?update=yes"))
                                            AddFile(image3.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                                    }

                                    //Specifications
                                    product.SpecificationValues.Clear();
                                    string[] Specs = specifications.Split(';');
                                    foreach (var item in Specs)
                                    {
                                        if (item != String.Empty)
                                        {
                                            string[] specificationValue = item.Split(':');

                                            if (!_productOptionManager.AnyExistingSpecificationAttributesWithName(specificationValue[0]))
                                            {
                                                _productOptionManager.AddSpecificationAttribute(new ProductSpecificationAttribute() { Name = specificationValue[0] });
                                            }

                                            ProductSpecificationAttribute option = _productOptionManager.GetSpecificationAttributeByName(specificationValue[0]);
                                            if (product.SpecificationValues.Where(x => x.ProductSpecificationAttribute.Id == option.Id && x.Product.Id == product.Id).Count() == 0)
                                                product.SpecificationValues.Add(new ProductSpecificationValue() { ProductSpecificationAttribute = option, Value = specificationValue[1], Product = product });
                                            if (!option.Options.Where(x => x.Name == specificationValue[1]).Any())
                                            {
                                                option.Options.Add(new ProductSpecificationAttributeOption() { ProductSpecificationAttribute = option, Name = specificationValue[1] });
                                                _productOptionManager.UpdateSpecificationAttribute(option);
                                            }
                                        }
                                    }
                                   
                                    _documentService.SaveDocument<Product>(product);

                                    ProductVariant productVariant = _productVariantService.GetProductVariantBySKU(sku);
                                    if (productVariant == null)
                                    {
                                        productVariant = new ProductVariant();
                                    }
                                    productVariant.Name = variantName;
                                    productVariant.SKU = sku;
                                    productVariant.Barcode = barcode;
                                    productVariant.BasePrice = GeneralHelper.ChangeTypeFromString<decimal>(price);
                                    productVariant.PreviousPrice = GeneralHelper.ChangeTypeFromString<decimal>(previousPrice);
                                    productVariant.StockRemaining = GeneralHelper.ChangeTypeFromString<int>(stock);
                                    productVariant.Weight = GeneralHelper.ChangeTypeFromString<decimal>(weight);
                                    if (trackingPolicy == "Track")
                                        productVariant.TrackingPolicy = TrackingPolicy.Track;
                                    else
                                        productVariant.TrackingPolicy = TrackingPolicy.DontTrack;
                                    if (GeneralHelper.ChangeTypeFromString<int>(taxRate) != 0)
                                    {
                                        productVariant.TaxRate = _taxRateManager.Get(GeneralHelper.ChangeTypeFromString<int>(taxRate));
                                    }

                                    //Save or Update
                                    productVariant.Product = product;
                                    productVariant.AttributeValues.Clear();
                                    _productVariantService.Update(productVariant);
                                    variantsInImportedFile.Add(productVariant);

                                    productVariant = _productVariantService.GetProductVariantBySKU(sku);

                                    //Options
                                    if (!String.IsNullOrWhiteSpace(option1Name) && !String.IsNullOrWhiteSpace(option1Value))
                                    {
                                        if (!_productOptionManager.AnyExistingAttributeOptionsWithName(option1Name))
                                            _productOptionManager.AddAttributeOption(new ProductAttributeOption() { Name = option1Name });
                                        var option = _productOptionManager.GetAttributeOptionByName(option1Name);
                                        if (productVariant.Product.AttributeOptions.Where(x => x.Id == option.Id).Count() == 0)
                                            product.AttributeOptions.Add(option);
                                        if (productVariant.AttributeValues.Where(x => x.ProductAttributeOption.Id == option.Id).Count() == 0)
                                        {
                                            productVariant.AttributeValues.Add(new ProductAttributeValue()
                                            {
                                                ProductAttributeOption = option,
                                                ProductVariant = productVariant,
                                                Value = option1Value
                                            });
                                        }
                                    }
                                    if (!String.IsNullOrWhiteSpace(option2Name) && !String.IsNullOrWhiteSpace(option2Value))
                                    {
                                        if (!_productOptionManager.AnyExistingAttributeOptionsWithName(option2Name))
                                            _productOptionManager.AddAttributeOption(new ProductAttributeOption() { Name = option2Name });
                                        var option = _productOptionManager.GetAttributeOptionByName(option2Name);
                                        if (productVariant.Product.AttributeOptions.Where(x => x.Id == option.Id).Count() == 0)
                                            product.AttributeOptions.Add(option);
                                        if (productVariant.AttributeValues.Where(x => x.ProductAttributeOption.Id == option.Id).Count() == 0)
                                        {
                                            productVariant.AttributeValues.Add(new ProductAttributeValue()
                                            {
                                                ProductAttributeOption = option,
                                                ProductVariant = productVariant,
                                                Value = option2Value
                                            });
                                        }
                                    }
                                    if (!String.IsNullOrWhiteSpace(option3Name) && !String.IsNullOrWhiteSpace(option3Value))
                                    {
                                        if (!_productOptionManager.AnyExistingAttributeOptionsWithName(option3Name))
                                            _productOptionManager.AddAttributeOption(new ProductAttributeOption() { Name = option3Name });
                                        var option = _productOptionManager.GetAttributeOptionByName(option3Name);
                                        if (productVariant.Product.AttributeOptions.Where(x => x.Id == option.Id).Count() == 0)
                                            product.AttributeOptions.Add(option);
                                        if (productVariant.AttributeValues.Where(x => x.ProductAttributeOption.Id == option.Id).Count() == 0)
                                        {
                                            productVariant.AttributeValues.Add(new ProductAttributeValue()
                                            {
                                                ProductAttributeOption = option,
                                                ProductVariant = productVariant,
                                                Value = option3Value
                                            });
                                        }
                                    }
                                    _productVariantService.Update(productVariant);
                                    lastAddedProductID = 0;
                                }
                                foreach (var item in _productVariantService.GetAll())
                                {
                                    if (!variantsInImportedFile.Where(x => x.Id == item.Id).Any())
                                        _productVariantService.Delete(item);
                                }
                                messages.Add("All products and variants were successfully imported.");
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
            //}
            //catch (Exception)
            //{
            //    messages.Add("Error reading file. It is possible that file is corrupted.");
            //}
            return messages;
        }

        private void AddFile(string fileLocation, MediaCategory mediaCategory)
        {
            using (WebClient client = new WebClient())
            {
                string fileName = Path.GetFileName(fileLocation);
                string fileExt = Path.GetExtension(fileLocation);
                var downloadedFile = client.DownloadData(fileLocation);
                _fileService.AddFile(new MemoryStream(downloadedFile), fileName, "image/png", downloadedFile.Length, mediaCategory);
            }
        }

        private string GetValueFromRow(ExcelPackage excelFile, int worksheetId, int rowId, int cellId)
        {
            if (excelFile.Workbook.Worksheets[worksheetId].Cells[rowId, cellId] != null && excelFile.Workbook.Worksheets[worksheetId].Cells[rowId, cellId].Value != null)
                return excelFile.Workbook.Worksheets[worksheetId].Cells[rowId, cellId].Value.ToString();
            else
                return String.Empty;
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

                IList<ProductVariant> productVariants = _productVariantService.GetAll();

                for (int i = 0; i < productVariants.Count; i++)
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
                            wsProducts.Cells["J" + rowId].Value += item.ProductSpecificationAttribute.Name + ":" + item.Value + ";";
                        }
                    }
                    wsProducts.Cells["K" + rowId].Value = productVariants[i].Name!=null?productVariants[i].Name:String.Empty;
                    wsProducts.Cells["L" + rowId].Value = productVariants[i].BasePrice;
                    wsProducts.Cells["M" + rowId].Value = productVariants[i].PreviousPrice;
                    if (productVariants[i].TaxRate != null)
                        wsProducts.Cells["N" + rowId].Value = productVariants[i].TaxRate.Id;
                    wsProducts.Cells["O" + rowId].Value = productVariants[i].Weight;
                    wsProducts.Cells["P" + rowId].Value = productVariants[i].StockRemaining;
                    wsProducts.Cells["Q" + rowId].Value = productVariants[i].TrackingPolicy;
                    wsProducts.Cells["R" + rowId].Value = productVariants[i].SKU;
                    wsProducts.Cells["S" + rowId].Value = productVariants[i].Barcode;

                    for (int v = 0; v < productVariants[i].AttributeValues.OrderBy(x => x.ProductAttributeOption.DisplayOrder).Count(); v++)
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

                    if (productVariants[i].Product.Images.Any())
                    {
                        wsProducts.Cells["Z" + rowId].Value = "http://"+CurrentRequestData.CurrentSite.BaseUrl+productVariants[i].Product.Images.First().FileUrl+"?update=no";
                        if(productVariants[i].Product.Images.Count()>1)
                            wsProducts.Cells["AA" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.ToList()[1].FileUrl + "?update=no";
                        if (productVariants[i].Product.Images.Count() > 2)
                            wsProducts.Cells["AB" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.ToList()[2].FileUrl + "?update=no";
                    }
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
    }
}