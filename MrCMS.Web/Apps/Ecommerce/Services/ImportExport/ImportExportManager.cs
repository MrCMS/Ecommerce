using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using NHibernate;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using System.Net;
using System.IO;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Ecommerce.Models;

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
        private readonly IFileService _fileService;
        private readonly IIndexService _indexService;

        public ImportExportManager(ISession session, IProductService productService, IProductVariantService productVariantService,
            IDocumentService documentService, IProductOptionManager productOptionManager, IBrandService brandService, ITaxRateManager taxRateManager,
            IFileService fileService, IIndexService indexService)
        {
            _session = session;
            _productService = productService;
            _productVariantService = productVariantService;
            _documentService = documentService;
            _productOptionManager = productOptionManager;
            _brandService = brandService;
            _taxRateManager = taxRateManager;
            _fileService = fileService;
            _indexService = indexService;
        }
        
        public Dictionary<string, List<string>> ImportProductsFromExcel(HttpPostedFileBase file)
        {
            var spreadsheet = new ExcelPackage(file.InputStream);

            Dictionary<string, List<string>> parseErrors;
            var productsToImport = GetProductsFromSpreadSheet(spreadsheet, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            var businessLogicErrors = ValidateBusinessLogic(productsToImport);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            ImportProductsFromDTOs(productsToImport);
            return new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Do import
        /// </summary>
        /// <param name="productsToImport"></param>
        private void ImportProductsFromDTOs(List<ProductImportDataTransferObject> productsToImport)
        {
            foreach (var dataTransferObject in productsToImport)
            {
                Import(dataTransferObject);
            }
        }

        private void Import(ProductImportDataTransferObject dataTransferObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Apply business logic here
        /// </summary>
        /// <param name="productsToImport"></param>
        /// <returns></returns>
        private Dictionary<string,List<string>> ValidateBusinessLogic(List<ProductImportDataTransferObject> productsToImport)
        {
            var errors = new Dictionary<string, List<string>>();
            var rules = MrCMSApplication.GetAll<IProductImportValidationRule>();

            foreach (var dataTransferObject in productsToImport)
            {
                var productErrors = rules.SelectMany(rule => rule.GetErrors(dataTransferObject)).ToList();
                if (productErrors.Any())
                    errors.Add(dataTransferObject.UrlSegment, productErrors);
            }

            return errors;
        }

        /// <summary>
        /// Try and get data out of the spreadsheet into the DTOs with parse and type checks
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private List<ProductImportDataTransferObject> GetProductsFromSpreadSheet(ExcelPackage spreadsheet, out Dictionary<string, List<string>> parseErrors)
        {
            var productsToImport = new List<ProductImportDataTransferObject>();
            parseErrors = new Dictionary<string, List<string>>();

            parseErrors.Add("file", new List<string>());
            if (spreadsheet == null)
                parseErrors["file"].Add("No import file");
            else
            {
                if (spreadsheet.Workbook == null)
                    parseErrors["file"].Add("Error reading Workbook from import file.");
                else
                {
                    if (spreadsheet.Workbook.Worksheets.Count == 0)
                        parseErrors["file"].Add("No worksheets in import file.");
                    else
                    {
                        if (spreadsheet.Workbook.Worksheets.Count<2 && (!spreadsheet.Workbook.Worksheets.Where(x=>x.Name=="Info").Any() ||
                            !spreadsheet.Workbook.Worksheets.Where(x => x.Name == "Products").Any()))
                            parseErrors["file"].Add("One or both of the required worksheets (Info and Products) are not present in import file.");
                    }
                }
            }

            parseErrors = parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);

            if (!parseErrors.Any())
            {
                var worksheet = spreadsheet.Workbook.Worksheets.Where(x => x.Name == "Products").SingleOrDefault();
                var totalRows = worksheet.Dimension.End.Row;
                for (var rowId = 2; rowId <= totalRows; rowId++)
                {
                    var product = new ProductImportDataTransferObject();

                    //Prepare handle name for storing and grouping errors
                    string handle = String.Empty, url = worksheet.GetValue<string>(rowId, 1), name = worksheet.GetValue<string>(rowId, 2);
                    if (url.HasValue())
                        handle = url;
                    else
                        handle = name;

                    if (!parseErrors.Any(x => x.Key == handle))
                    {
                        parseErrors.Add(handle, new List<string>());
                        product.UrlSegment = worksheet.GetValue<string>(rowId, 1);
                        if (worksheet.GetValue<string>(rowId, 2).HasValue())
                            product.Name = worksheet.GetValue<string>(rowId, 2);
                        else
                            parseErrors[handle].Add("Product Name is required.");
                        product.Description = worksheet.GetValue<string>(rowId, 3);
                        product.SEOTitle = worksheet.GetValue<string>(rowId, 4);
                        product.SEODescription = worksheet.GetValue<string>(rowId, 5);
                        product.SEOKeywords = worksheet.GetValue<string>(rowId, 6);
                        product.Abstract = worksheet.GetValue<string>(rowId, 7);
                        product.Brand = worksheet.GetValue<string>(rowId, 8);

                        //Categories
                        try
                        {
                            string[] Cats = worksheet.GetValue<string>(rowId, 9).Split(';');
                            foreach (var item in Cats)
                            {
                                if (!String.IsNullOrWhiteSpace(item))
                                {
                                    int catId = 0;
                                    Int32.TryParse(item, out catId);
                                    product.Categories.Add(catId);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            parseErrors[handle].Add("Product Categories field value contains illegal characters / not in correct format.");
                        }

                        //Specifications
                        if (!String.IsNullOrWhiteSpace(worksheet.GetValue<string>(rowId, 10)))
                        {
                            try
                            {
                                if (!worksheet.GetValue<string>(rowId, 10).Contains(":"))
                                    parseErrors[handle].Add("Product Specifications field value contains illegal characters / not in correct format. Names and Values (Item) must be split with :, and items must be split by ;");
                                string[] Specs = worksheet.GetValue<string>(rowId, 10).Split(';');
                                foreach (var item in Specs)
                                {
                                    if (!String.IsNullOrWhiteSpace(item))
                                    {
                                        string[] specificationValue = item.Split(':');
                                        if (!String.IsNullOrWhiteSpace(specificationValue[0]) && !String.IsNullOrWhiteSpace(specificationValue[1]))
                                            product.Specifications.Add(specificationValue[0], specificationValue[1]);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                parseErrors[handle].Add("Product Specifications field value contains illegal characters / not in correct format. Names and Values (Item) must be split with :, and items must be split by ;");
                            }
                        }

                        //Images
                        product.Image1 = worksheet.GetValue<string>(rowId, 26);
                        product.Image2 = worksheet.GetValue<string>(rowId, 27);
                        product.Image3 = worksheet.GetValue<string>(rowId, 28);

                        productsToImport.Add(product);
                    }
                    else
                        product = productsToImport.Where(x => x.Name == name && x.UrlSegment == url).SingleOrDefault();

                    //Variants
                    if (product != null)
                    {
                        var productVariant = new ProductVariantImportDataTransferObject();

                        productVariant.Name = worksheet.GetValue<string>(rowId,11);
                        if (!worksheet.GetValue<string>(rowId, 12).IsConvertable<decimal>())
                            parseErrors[handle].Add("Price value is not a valid decimal number.");
                        else if (worksheet.GetValue<string>(rowId,12).HasValue())
                            productVariant.Price = worksheet.GetValue<decimal>(rowId, 12);
                        else
                            parseErrors[handle].Add("Price is required.");
                        if (!worksheet.GetValue<string>(rowId, 13).IsConvertable<decimal>())
                            parseErrors[handle].Add("Previous Price value is not a valid decimal number.");
                        else
                            productVariant.PreviousPrice = worksheet.GetValue<decimal?>(rowId, 13);
                        if (!worksheet.GetValue<string>(rowId, 14).IsConvertable<int>())
                            parseErrors[handle].Add("Tax Rate Id value is not a valid number.");
                        else
                            productVariant.TaxRate = worksheet.GetValue<int>(rowId, 14);
                        if (!worksheet.GetValue<string>(rowId, 15).IsConvertable<int>())
                            parseErrors[handle].Add("Weight value is not a valid decimal number.");
                        else
                            productVariant.Weight = worksheet.GetValue<decimal?>(rowId, 15);
                        if (!worksheet.GetValue<string>(rowId, 16).IsConvertable<int>())
                            parseErrors[handle].Add("Stock value is not a valid decimal number.");
                        else
                            productVariant.Stock = worksheet.GetValue<int>(rowId, 16);
                        if (!worksheet.GetValue<string>(rowId, 17).HasValue() ||
                            (worksheet.GetValue<string>(rowId, 17) != "Track" &&
                            worksheet.GetValue<string>(rowId, 17) != "DontTrack"))
                            parseErrors[handle].Add("Tracking Policy must have either 'Track' or 'DontTrack' value.");
                        else
                        {
                            if (worksheet.GetValue<string>(rowId, 17) == "Track")
                                productVariant.TrackingPolicy = TrackingPolicy.Track;
                            else
                                productVariant.TrackingPolicy = TrackingPolicy.DontTrack;
                        }
                        if (worksheet.GetValue<string>(rowId, 18).HasValue())
                            productVariant.SKU = worksheet.GetValue<string>(rowId, 18);
                        else
                            parseErrors[handle].Add("SKU is required.");
                        productVariant.Barcode = worksheet.GetValue<string>(rowId, 19);
                        if (worksheet.GetValue<string>(rowId, 20).HasValue() && worksheet.GetValue<string>(rowId, 21).HasValue())
                            productVariant.Options.Add(worksheet.GetValue<string>(rowId, 20), worksheet.GetValue<string>(rowId, 21));
                        if (worksheet.GetValue<string>(rowId, 22).HasValue() && worksheet.GetValue<string>(rowId, 23).HasValue())
                            productVariant.Options.Add(worksheet.GetValue<string>(rowId, 22), worksheet.GetValue<string>(rowId, 23));
                        if (worksheet.GetValue<string>(rowId, 24).HasValue() && worksheet.GetValue<string>(rowId, 25).HasValue())
                            productVariant.Options.Add(worksheet.GetValue<string>(rowId, 24), worksheet.GetValue<string>(rowId, 25));

                        product.ProductVariants.Add(productVariant);
                    }
                }

                //Remove handles with no errors
                parseErrors = parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);

                //Remove duplicate errors
                parseErrors = parseErrors.GroupBy(pair => pair.Value).Select(group => group.First()).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return productsToImport;
        }
       
        private void AddProductImage(string fileLocation, MediaCategory mediaCategory)
        {
            using (WebClient client = new WebClient())
            {
                string fileName = Path.GetFileName(fileLocation);
                string fileExt = Path.GetExtension(fileLocation);
                var downloadedFile = client.DownloadData(fileLocation);
                _fileService.AddFile(new MemoryStream(downloadedFile), fileName, "image/png", downloadedFile.Length, mediaCategory);
            }
        }

        //EXPORT
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
                            wsProducts.Cells["J" + rowId].Value += item.SpecificationName + ":" + item.Value + ";";
                        }
                    }
                    wsProducts.Cells["K" + rowId].Value = productVariants[i].Name != null ? productVariants[i].Name : String.Empty;
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
                        wsProducts.Cells["Z" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl + productVariants[i].Product.Images.First().FileUrl + "?update=no";
                        if (productVariants[i].Product.Images.Count() > 1)
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