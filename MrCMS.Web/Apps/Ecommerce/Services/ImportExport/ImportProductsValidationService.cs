using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules;
using MrCMS.Website;
using OfficeOpenXml;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductsValidationService : IImportProductsValidationService
    {
        private readonly IDocumentService _documentService;

        public ImportProductsValidationService(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Apply business logic here
        /// </summary>
        /// <param name="productsToImport"></param>
        /// <returns></returns>
        public Dictionary<string,List<string>> ValidateBusinessLogic(IEnumerable<ProductImportDataTransferObject> productsToImport)
        {
            var errors = new Dictionary<string, List<string>>();
            var productRules = MrCMSApplication.GetAll<IProductImportValidationRule>();
            var productVariantRules = MrCMSApplication.GetAll<IProductVariantImportValidationRule>();

            foreach (var product in productsToImport)
            {
                var productErrors = productRules.SelectMany(rule => rule.GetErrors(product)).ToList();
                if (productErrors.Any())
                    errors.Add(product.UrlSegment, productErrors);

                foreach (var variant in product.ProductVariants)
                {
                    var productVariantErrors = productVariantRules.SelectMany(rule => rule.GetErrors(variant)).ToList();
                    if (productVariantErrors.Any())
                    {
                        if (!errors.Any(x => x.Key == product.UrlSegment))
                            errors.Add(product.UrlSegment, productVariantErrors);
                        else
                            errors[product.UrlSegment].AddRange(productVariantErrors);
                    }
                        
                }
            }

            return errors;
        }

        public List<ProductImportDataTransferObject> ValidateAndImportProductsWithVariants(ExcelPackage spreadsheet, ref Dictionary<string, List<string>> parseErrors)
        {
            var productsToImport = new List<ProductImportDataTransferObject>();

            if (!parseErrors.Any())
            {
                if (spreadsheet != null)
                {
                    if (spreadsheet.Workbook != null)
                    {
                        var worksheet = spreadsheet.Workbook.Worksheets.SingleOrDefault(x => x.Name == "Items");
                        if (worksheet != null)
                        {
                            var totalRows = worksheet.Dimension.End.Row;
                            for (var rowId = 2; rowId <= totalRows; rowId++)
                            {
                                var product = new ProductImportDataTransferObject();

                                //Prepare handle name for storing and grouping errors
                                string url = worksheet.GetValue<string>(rowId, 1), name = worksheet.GetValue<string>(rowId, 2);
                                var handle = url.HasValue() ? url : name;

                                if (!productsToImport.Any(x => x.Name == name || x.UrlSegment == url))
                                {
                                    if (!parseErrors.Any(x => x.Key == handle))
                                        parseErrors.Add(handle, new List<string>());
                                    product.UrlSegment = worksheet.GetValue<string>(rowId, 1).HasValue()
                                                             ? worksheet.GetValue<string>(rowId, 1)
                                                             : _documentService.GetDocumentUrl(name, null);
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
                                        var value = worksheet.GetValue<string>(rowId, 9);
                                        if (!String.IsNullOrWhiteSpace(value))
                                        {
                                            var Cats = value.Split(';');
                                            foreach (var item in Cats)
                                            {
                                                if (!String.IsNullOrWhiteSpace(item))
                                                {
                                                    var catId = 0;
                                                    Int32.TryParse(item, out catId);
                                                    product.Categories.Add(catId);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        parseErrors[handle].Add(
                                            "Product Categories field value contains illegal characters / not in correct format.");
                                    }

                                    //Specifications
                                    if (!String.IsNullOrWhiteSpace(worksheet.GetValue<string>(rowId, 10)))
                                    {
                                        try
                                        {
                                            var value = worksheet.GetValue<string>(rowId, 10);
                                            if (!String.IsNullOrWhiteSpace(value))
                                            {
                                                if (!worksheet.GetValue<string>(rowId, 10).Contains(":"))
                                                    parseErrors[handle].Add(
                                                        "Product Specifications field value contains illegal characters / not in correct format. Names and Values (Item) must be split with :, and items must be split by ;");
                                                var specs = value.Split(';');
                                                foreach (var item in specs)
                                                {
                                                    if (!String.IsNullOrWhiteSpace(item))
                                                    {
                                                        string[] specificationValue = item.Split(':');
                                                        if (!String.IsNullOrWhiteSpace(specificationValue[0]) &&
                                                            !String.IsNullOrWhiteSpace(specificationValue[1]))
                                                            product.Specifications.Add(specificationValue[0],
                                                                                       specificationValue[1]);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            parseErrors[handle].Add(
                                                "Product Specifications field value contains illegal characters / not in correct format. Names and Values (Item) must be split with :, and items must be split by ;");
                                        }
                                    }

                                    //Images
                                    if (worksheet.GetValue<string>(rowId, 26).HasValue())
                                        product.Images.Add(worksheet.GetValue<string>(rowId, 26));
                                    if (worksheet.GetValue<string>(rowId, 27).HasValue())
                                        product.Images.Add(worksheet.GetValue<string>(rowId, 27));
                                    if (worksheet.GetValue<string>(rowId, 28).HasValue())
                                        product.Images.Add(worksheet.GetValue<string>(rowId, 28));

                                    productsToImport.Add(product);
                                }
                                else
                                    product = productsToImport.SingleOrDefault(x => x.Name == name && x.UrlSegment == url);

                                //Variants
                                if (product != null)
                                {
                                    var productVariant = new ProductVariantImportDataTransferObject
                                    {
                                        Name = worksheet.GetValue<string>(rowId, 11)
                                    };

                                    if (!worksheet.GetValue<string>(rowId, 12).IsValidInput<decimal>())
                                        parseErrors[handle].Add("Price value is not a valid decimal number.");
                                    else if (worksheet.GetValue<string>(rowId, 12).HasValue())
                                        productVariant.Price = worksheet.GetValue<decimal>(rowId, 12);
                                    else
                                        parseErrors[handle].Add("Price is required.");
                                    if (!worksheet.GetValue<string>(rowId, 13).IsValidInput<decimal>())
                                        parseErrors[handle].Add("Previous Price value is not a valid decimal number.");
                                    else
                                        productVariant.PreviousPrice = worksheet.GetValue<decimal>(rowId, 13);
                                    if (!worksheet.GetValue<string>(rowId, 14).IsValidInput<int>())
                                        parseErrors[handle].Add("Tax Rate Id value is not a valid number.");
                                    else
                                        productVariant.TaxRate = worksheet.GetValue<int>(rowId, 14);
                                    if (!worksheet.GetValue<string>(rowId, 15).IsValidInput<decimal>())
                                        parseErrors[handle].Add("Weight value is not a valid decimal number.");
                                    else
                                        productVariant.Weight = worksheet.GetValue<decimal>(rowId, 15);
                                    if (!worksheet.GetValue<string>(rowId, 16).IsValidInput<int>())
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
                                    if (worksheet.GetValue<string>(rowId, 20).HasValue() &&
                                        worksheet.GetValue<string>(rowId, 21).HasValue())
                                        productVariant.Options.Add(worksheet.GetValue<string>(rowId, 20),
                                                                   worksheet.GetValue<string>(rowId, 21));
                                    if (worksheet.GetValue<string>(rowId, 22).HasValue() &&
                                        worksheet.GetValue<string>(rowId, 23).HasValue())
                                        productVariant.Options.Add(worksheet.GetValue<string>(rowId, 22),
                                                                   worksheet.GetValue<string>(rowId, 23));
                                    if (worksheet.GetValue<string>(rowId, 24).HasValue() &&
                                        worksheet.GetValue<string>(rowId, 25).HasValue())
                                        productVariant.Options.Add(worksheet.GetValue<string>(rowId, 24),
                                                                   worksheet.GetValue<string>(rowId, 25));

                                    product.ProductVariants.Add(productVariant);
                                }
                            }
                        }
                    }
                }

                //Remove handles with no errors
                parseErrors = parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return productsToImport;
        }

        public Dictionary<string, List<string>> ValidateImportFile(ExcelPackage spreadsheet)
        {
            var parseErrors = new Dictionary<string, List<string>> { { "file", new List<string>() } };

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
                        if (spreadsheet.Workbook.Worksheets.Count < 2 ||
                            !spreadsheet.Workbook.Worksheets.Any(x => x.Name == "Info") ||
                             !spreadsheet.Workbook.Worksheets.Any(x => x.Name == "Items"))
                            parseErrors["file"].Add(
                                "One or both of the required worksheets (Info and Items) are not present in import file.");
                    }
                }
            }

            return parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}