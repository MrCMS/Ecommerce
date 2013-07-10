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

        private ExcelPackage File { get; set; }
        private ExcelWorksheet Products { get; set; }
        private Dictionary<string, List<string>> FileErrors
        {
            get
            {
                var errors = new Dictionary<string, List<string>>();

                errors.Add("file", new List<string>());

                if (File == null)
                    errors["file"].Add("No import file");
                else
                {
                    if (File.Workbook == null)
                        errors["file"].Add("Error reading Workbook from import file.");
                    else
                    {
                        if (File.Workbook.Worksheets.Count == 0)
                            errors["file"].Add("No worksheets in import file.");
                        else
                        {
                            if (File.Workbook.Worksheets[1].Name != "Info" &&
                                File.Workbook.Worksheets[2].Name != "Products")
                                errors["file"].Add("Required 'Info' and 'Products' worksheets are not present in import file.");
                        }
                    }
                }

                errors = errors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);

                return errors;
            }
        }
        private Dictionary<string, List<string>> ValidationErrors { get; set; }
        private List<int> ValidRows { get; set; }

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
        
        public Dictionary<string, List<string>> ImportProductsFromExcel_GARY(HttpPostedFileBase file)
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
                    errors.Add(dataTransferObject.Url, productErrors);
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
            throw new NotImplementedException();
        }
        
        public Dictionary<string, List<string>> ImportProductsFromExcel(HttpPostedFileBase file)
        {
            File = new ExcelPackage(file.InputStream);
            Validate();
            if (FileErrors.Any())
                return FileErrors;
            return ValidationErrors;
        }

        private void Validate()
        {
            ValidRows = new List<int>();
            ValidationErrors = new Dictionary<string, List<string>>();

            if (!FileErrors.Any())
            {
                Products = File.Workbook.Worksheets[2];
                var totalRows = File.Workbook.Worksheets[2].Dimension.End.Row;
                for (var rowId = 2; rowId <= totalRows; rowId++)
                {
                    string handle = String.Empty;
                    if (Products.GetValue<string>(rowId, 1).HasValue())
                        handle = Products.GetValue<string>(rowId, 1);
                    else if (Products.GetValue<string>(rowId, 2).HasValue())
                        handle = Products.GetValue<string>(rowId, 2);
                    else
                        handle = Products.GetValue<string>(rowId, 11);

                    if(!ValidationErrors.Any(x=>x.Key==handle))
                        ValidationErrors.Add(handle, new List<string>());

                    if (!Products.GetValue<string>(rowId, 2).HasValue())
                        ValidationErrors[handle].Add("Product Name is required.");
                    else
                        ValidateStringField(handle, rowId, 2, "Product Name", 255);
                    ValidateStringField(handle, rowId, 4, "Product SEO Title", 250);
                    ValidateStringField(handle, rowId, 5, "Product SEO Description", 250);
                    ValidateStringField(handle, rowId, 6, "Product SEO Keywords", 250);
                    ValidateStringField(handle, rowId, 7, "Product Abstract", 500);
                    ValidateStringField(handle, rowId, 8, "Product Brand", 255);
                    ValidateCategories(handle, rowId, 9);
                    ValidateSpecifications(handle, rowId, 10);
                    ValidateImageField(handle, rowId, 26, "Product Image 1");
                    ValidateImageField(handle, rowId, 27, "Product Image 2");
                    ValidateImageField(handle, rowId, 28, "Product Image 3");

                    string variantHandle = String.Empty;
                    if (!Products.GetValue<string>(rowId, 18).HasValue())
                    {
                        if (!Products.GetValue<string>(rowId, 11).HasValue())
                            variantHandle = " (Name:" + Products.GetValue<string>(rowId, 11) + ")";
                        ValidationErrors[handle].Add("Product Variant SKU " + variantHandle + " is required.");
                    }
                    else
                    {
                        variantHandle = " (SKU:" + Products.GetValue<string>(rowId, 18) + ")";
                        ValidateStringField(handle, rowId, 18, "Product Variant SKU" + variantHandle, 255);
                    }
                    ValidateStringField(handle, rowId, 11, "Product Variant Name" + variantHandle, 255);
                    if (!Products.GetValue<string>(rowId, 12).HasValue())
                        ValidationErrors[handle].Add("Product Variant Price"+variantHandle+" is required.");
                    else
                        ValidateDecimalField(handle, rowId, 12, "Product Variant Price" ,variantHandle,true);
                    ValidateDecimalField(handle, rowId, 13,"Product Variant Previous Price" ,variantHandle,false);
                    ValidateTax(handle, rowId, 14, variantHandle);
                    ValidateDecimalField(handle, rowId, 15, "Product Variant Weight", variantHandle,false);
                    ValidateStock(handle, rowId, 16, variantHandle);
                    ValidateTrackingPolicy(handle, rowId, 17, variantHandle);
                    ValidateStringField(handle, rowId, 19, "Product Variant Barcode" + variantHandle, 14);
                    ValidateStringField(handle, rowId, 20, "Product Variant Option 1 Name" + variantHandle, 255);
                    ValidateStringField(handle, rowId, 21, "Product Variant Option 1 Value" + variantHandle, 255);
                    ValidateStringField(handle, rowId, 22, "Product Variant Option 2 Name" + variantHandle, 255);
                    ValidateStringField(handle, rowId, 23, "Product Variant Option 2 Value" + variantHandle, 255);
                    ValidateStringField(handle, rowId, 24, "Product Variant Option 3 Name" + variantHandle, 255);
                    ValidateStringField(handle, rowId, 25, "Product Variant Option 3 Value" + variantHandle, 255);

                    if (ValidationErrors[handle].Count == 0)
                        ValidRows.Add(rowId);

                    ValidationErrors[handle] = ValidationErrors[handle].GroupBy(pair => pair)
                         .Select(group => group.First())
                         .ToList();
                }

                ValidationErrors = ValidationErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);

                ValidationErrors = ValidationErrors.GroupBy(pair => pair.Value)
                         .Select(group => group.First())
                         .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }
        private void ValidateStringField(string handle, int rowId, int colId, string fieldName, int maxLength)
        {
            if (!Products.GetValue<string>(rowId, colId).IsValidLength(maxLength))
                ValidationErrors[handle].Add(fieldName + " value cannot have more than " + maxLength + " characters in length.");
        }
        private void ValidateImageField(string handle, int rowId, int colId, string fieldName)
        {
            if (!Products.GetValue<string>(rowId, colId).IsValidUrl())
                ValidationErrors[handle].Add(fieldName + " value is not valid url.");
        }
        private void ValidateTrackingPolicy(string handle, int rowId, int colId, string variantHandle)
        {
            if (!Products.GetValue<string>(rowId, colId).HasValue() || (Products.GetValue<string>(rowId, colId) != "Track" && Products.GetValue<string>(rowId, colId) != "DontTrack"))
                ValidationErrors[handle].Add("Product Variant Tracking Policy"+variantHandle+" must have either 'Track' or 'DontTrack' value.");
        }
        private void ValidateTax(string handle, int rowId, int colId, string variantHandle)
        {
            if (!Products.GetValue<string>(rowId, colId).HasValue() && MrCMS.Website.MrCMSApplication.Get<MrCMS.Web.Apps.Ecommerce.Settings.TaxSettings>().TaxesEnabled)
            {
                if (!Products.GetValue<string>(rowId, colId).HasValue() && Products.GetValue<int>(rowId, colId) == 0)
                {
                    ValidationErrors[handle].Add("Product Variant Tax Rate is required (because Taxes are enabled inside MrCMS).");
                }
                else
                {
                    if (_taxRateManager.Get(Products.GetValue<int>(rowId, colId)) == null)
                        ValidationErrors[handle].Add("Product Variant Tax Rate" + variantHandle + " value is not valid (doesn't exist).");
                }
            }
            else
            {
                if (Products.GetValue<string>(rowId, colId).HasValue() && Products.GetValue<int>(rowId, colId)!=0)
                {
                    if(_taxRateManager.Get(Products.GetValue<int>(rowId, colId))==null)
                        ValidationErrors[handle].Add("Product Variant Tax Rate" + variantHandle + " value is not valid (doesn't exist).");
                }
            }
        }
        private void ValidateDecimalField(string handle, int rowId, int colId,string fieldName, string variantHandle, bool isRequired)
        {
            if (isRequired)
            {
                if (!Products.GetValue<decimal?>(rowId, colId).HasValue)
                    ValidationErrors[handle].Add(fieldName + variantHandle + " is required.");
                else
                {
                    if (Products.GetValue<decimal?>(rowId, colId).Value < 0)
                        ValidationErrors[handle].Add(fieldName + variantHandle + " must have value greater than or equal to 0.");
                }
            }
            else
            {
                if (Products.GetValue<decimal?>(rowId, colId).HasValue && Products.GetValue<decimal?>(rowId, colId).Value < 0)
                    ValidationErrors[handle].Add(fieldName + variantHandle + " must have value greater than or equal to 0.");
            }
        }
        private void ValidateStock(string handle, int rowId, int colId, string variantHandle)
        {
            if (Products.GetValue<string>(rowId, 17).HasValue() && 
                Products.GetValue<string>(rowId, 17) == "Track" &&
                !Products.GetValue<string>(rowId, colId).HasValue())
                ValidationErrors[handle].Add("Product Variant Stock Remaining" + variantHandle + " must have value if Tracking is enabled.");
        }
        private void ValidateCategories(string handle, int rowId, int colId)
        {
            try
            {
                string[] Cats = Products.GetValue<string>(rowId, colId).Split(';');
                foreach (var item in Cats)
                {
                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        int catId = 0;
                        Int32.TryParse(item, out catId);
                        if (_documentService.GetDocument<Category>(catId) == null)
                            ValidationErrors[handle].Add("Product Category with Id: '" + item + "' doesn't exist in system.");
                    }
                }
            }
            catch (Exception)
            {
                ValidationErrors[handle].Add("Product Categories field value contains illegal characters / Not in correct format - Items must be split by ;");
            }
        }
        private void ValidateSpecifications(string handle, int rowId, int colId)
        {
            if (!String.IsNullOrWhiteSpace(Products.GetValue<string>(rowId, colId)))
            {
                try
                {
                    if (!Products.GetValue<string>(rowId, colId).Contains(":"))
                        ValidationErrors[handle].Add("Product Specifications field value contains illegal characters / Not in correct format - Names and Values (Item) must be split with :, and items must be split by ;");
                    string[] Specs = Products.GetValue<string>(rowId, colId).Split(';');
                    foreach (var item in Specs)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            string[] specificationValue = item.Split(':');
                        }
                    }
                }
                catch (Exception)
                {
                    ValidationErrors[handle].Add("Product Specifications field value contains illegal characters / Not in correct format - Names and Values (Item) must be split with :, and items must be split by ;");
                }
            }
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