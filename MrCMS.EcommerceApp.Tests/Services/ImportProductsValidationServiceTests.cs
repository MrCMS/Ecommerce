using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;
using OfficeOpenXml;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Pages;
using EnumerableHelper = MrCMS.Helpers.EnumerableHelper;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportProductsValidationServiceTests : InMemoryDatabaseTest
    {
        private IDocumentService _documentService;
        private ImportProductsValidationService _importProductsValidationService;
       
        public ImportProductsValidationServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            A.Fake<IImportExportManager>();
            _importProductsValidationService= new ImportProductsValidationService(_documentService);
        }

        [Fact]
        public void ImportProductsValidationService_ValidateBusinessLogic_CallsAllIoCRegisteredRulesOnAllProducts()
        {
            var mockingKernel = new MockingKernel();
            var productImportValidationRules =
                Enumerable.Range(1, 10).Select(i => A.Fake<IProductImportValidationRule>()).ToList();
            var productVariantImportValidationRule = A.Fake<IProductVariantImportValidationRule>();
            productImportValidationRules.ForEach(rule => mockingKernel.Bind<IProductImportValidationRule>()
                                                                      .ToMethod(context => rule));
            mockingKernel.Bind<IProductVariantImportValidationRule>()
                         .ToMethod(context => productVariantImportValidationRule)
                         .InSingletonScope();
            MrCMSApplication.OverrideKernel(mockingKernel);

            var products = Enumerable.Range(1, 10).Select(i => new ProductImportDataTransferObject()).ToList();

            _importProductsValidationService.ValidateBusinessLogic(products);

            productImportValidationRules.ForEach(
                rule =>
                EnumerableHelper.ForEach(products, product => A.CallTo(() => rule.GetErrors(product)).MustHaveHappened()));
        }

        [Fact]
        public void ImportProductsValidationService_ValidateAndImportProductsWithVariants_ShouldReturnListOfProductsAndNoErrors()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            var products = _importProductsValidationService.ValidateAndImportProductsWithVariants(GetSpreadsheet(), ref parseErrors);

            products.Count.ShouldBeEquivalentTo(1);
            parseErrors.Count.ShouldBeEquivalentTo(0);
        }

        [Fact]
        public void ImportProductsValidationService_ValidateImportFile_ShouldReturnNoErrors()
        {
            var file = new FileInfo("test.xslx");
            var spreadsheet = new ExcelPackage(file);
            spreadsheet.Workbook.Worksheets.Add("Info");
            spreadsheet.Workbook.Worksheets.Add("Products");

            var errors = _importProductsValidationService.ValidateImportFile(spreadsheet);

            errors.Count.ShouldBeEquivalentTo(0);
        }

        private ExcelPackage GetSpreadsheet()
        {
            var product = new Product()
                {
                    Name = "Test",
                    UrlSegment = "test-url",
                    Abstract = "test",
                    BodyContent = "test",
                    MetaDescription = "test",
                    MetaTitle = "test",
                    MetaKeywords = "test"
                };
            var productVariants = new List<ProductVariant>()
                {
                    new ProductVariant()
                        {
                            Product = product,
                            Name = "Test Variant",
                            BasePrice = 1,
                            StockRemaining = 1,
                            TrackingPolicy = TrackingPolicy.Track,
                            Weight = 0,
                            SKU = "123"
                        }
                };

            var file = new FileInfo("test.xslx");
            var spreadsheet = new ExcelPackage(file);
            spreadsheet.Workbook.Worksheets.Add("Info");
            var wsProducts = spreadsheet.Workbook.Worksheets.Add("Products");

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

            for (var i = 0; i < productVariants.Count; i++)
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
                if (productVariants[i].Product.Categories.Any())
                {
                    foreach (var item in productVariants[i].Product.Categories)
                    {
                        wsProducts.Cells["I" + rowId].Value += item.Id + ";";
                    }
                }
                if (productVariants[i].Product.SpecificationValues.Any())
                {
                    foreach (var item in productVariants[i].Product.SpecificationValues)
                    {
                        wsProducts.Cells["J" + rowId].Value += item.SpecificationName + ":" + item.Value + ";";
                    }
                }
                wsProducts.Cells["K" + rowId].Value = productVariants[i].Name ?? String.Empty;
                wsProducts.Cells["L" + rowId].Value = productVariants[i].BasePrice;
                wsProducts.Cells["M" + rowId].Value = productVariants[i].PreviousPrice;
                if (productVariants[i].TaxRate != null)
                    wsProducts.Cells["N" + rowId].Value = productVariants[i].TaxRate.Id;
                wsProducts.Cells["O" + rowId].Value = productVariants[i].Weight;
                wsProducts.Cells["P" + rowId].Value = productVariants[i].StockRemaining;
                wsProducts.Cells["Q" + rowId].Value = productVariants[i].TrackingPolicy;
                wsProducts.Cells["R" + rowId].Value = productVariants[i].SKU;
                wsProducts.Cells["S" + rowId].Value = productVariants[i].Barcode;

                for (var v = 0;
                     v < productVariants[i].AttributeValues.OrderBy(x => x.ProductAttributeOption.DisplayOrder).Count();
                     v++)
                {
                    if (v == 0)
                    {
                        wsProducts.Cells["T" + rowId].Value =
                            productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                        wsProducts.Cells["U" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                    }
                    if (v == 1)
                    {
                        wsProducts.Cells["V" + rowId].Value =
                            productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                        wsProducts.Cells["W" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                    }
                    if (v == 2)
                    {
                        wsProducts.Cells["X" + rowId].Value =
                            productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                        wsProducts.Cells["Y" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                    }
                }

                if (!productVariants[i].Product.Images.Any()) continue;
                wsProducts.Cells["Z" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                                      productVariants[i].Product.Images.First().FileUrl + "?update=no";
                if (productVariants[i].Product.Images.Count() > 1)
                    wsProducts.Cells["AA" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                                           productVariants[i].Product.Images.ToList()[1].FileUrl +
                                                           "?update=no";
                if (productVariants[i].Product.Images.Count() > 2)
                    wsProducts.Cells["AB" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                                           productVariants[i].Product.Images.ToList()[2].FileUrl +
                                                           "?update=no";
            }

            return spreadsheet;
        }
    }
}