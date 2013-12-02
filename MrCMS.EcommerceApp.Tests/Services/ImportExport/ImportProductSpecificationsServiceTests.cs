using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportProductSpecificationsServiceTests : InMemoryDatabaseTest
    {
        private readonly ImportProductSpecificationsService _importSpecificationsService;

        public ImportProductSpecificationsServiceTests()
        {
            _importSpecificationsService = new ImportProductSpecificationsService(Session);
        }


        [Fact]
        public void ImportSpecificationsService_ImportSpecifications_ShouldAddANewSpecificationAttributeOptionIfItDoesntExist()
        {
            var productDTO = new ProductImportDataTransferObject
                                 {
                                     UrlSegment = "test-url",
                                     Specifications = new Dictionary<string, string>
                                                          {
                                                              {"Storage","16GB"}
                                                          }
                                 };

            var product = new Product() { Name = "Test Product" };
            _importSpecificationsService.Initialize();

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            _importSpecificationsService.ProductSpecificationAttributes.Should().HaveCount(1);
        }

        [Fact]
        public void ImportSpecificationsService_ImportSpecifications_ShouldAddANewSpecificationAttributeIfItDoesntExist()
        {
            var productDTO = new ProductImportDataTransferObject()
                                 {
                                     UrlSegment = "test-url",
                                     Specifications = new Dictionary<string, string>()
                                                          {
                                                              {"Storage","16GB"}
                                                          }
                                 };

            var product = new Product() { Name = "Test Product" };
            _importSpecificationsService.Initialize();

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            _importSpecificationsService.ProductSpecificationAttributes.First().Options.Should().HaveCount(1);
        }


        //[Fact]
        //public void ImportSpecificationsService_ImportVariantSpecifications_ShouldAddOptionsToProduct()
        //{
        //    var productVariantDTO = new ProductVariantImportDataTransferObject
        //                                {
        //                                    SKU = "123",
        //                                    Options = new Dictionary<string, string>() { { "Storage", "16GB" } }
        //                                };
        //    var productDTO = new ProductImportDataTransferObject
        //                         {
        //                             UrlSegment = "test-url",
        //                             ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
        //                         };

        //    var product = new Product() { Name = "Test Product" };
        //    var productVariant = new ProductVariant() { Name = "Test Product Variant", Product = product };

        //    var option = new ProductOption() { Id = 1, Name = "Storage" };
        //    A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).Returns(option);

        //    _importSpecificationsService.ImportVariantSpecifications(productVariantDTO, product, productVariant);

        //    product.Options.Should().HaveCount(1);
        //}
        //[Fact]
        //public void ImportSpecificationsService_ImportSpecifications_ShouldCallGetAttributeOptionByNameOfProductOptionManager()
        //{
        //    var productVariantDTO = new ProductVariantImportDataTransferObject()
        //                                {
        //                                    SKU = "123",
        //                                    Options = new Dictionary<string, string>() { { "Storage", "16GB" } }
        //                                };
        //    var productDTO = new ProductImportDataTransferObject()
        //                         {
        //                             UrlSegment = "test-url",
        //                             ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
        //                         };

        //    var product = new Product();
        //    var productVariant = new ProductVariant { Product = product };
        //    _importSpecificationsService.ImportVariantSpecifications(productVariantDTO, product, productVariant);

        //    A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).MustHaveHappened();
        //}

        [Fact]
        public void ImportSpecificationsService_ImportSpecifications_ShouldCallAnyExistingSpecificationAttributesWithNameOfProductOptionManager()
        {
            var productDTO = new ProductImportDataTransferObject()
                                 {
                                     UrlSegment = "test-url",
                                     Specifications = new Dictionary<string, string>() { { "Storage", "16GB" } }
                                 };

            var product = new Product();
            _importSpecificationsService.Initialize();

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            _importSpecificationsService.ProductSpecificationAttributes.First().Name.Should().Be("Storage");
        }

        [Fact]
        public void ImportSpecificationsService_ImportSpecifications_ShouldCallGetSpecificationAttributeByNameOfProductOptionManager()
        {
            var productDTO = new ProductImportDataTransferObject()
                                 {
                                     UrlSegment = "test-url",
                                     Specifications = new Dictionary<string, string>() { { "Storage", "16GB" } }
                                 };

            var product = new Product();
            _importSpecificationsService.Initialize();

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            _importSpecificationsService.ProductSpecificationAttributes.First().Options.First().Name.Should().Be("16GB");
        }

        [Fact]
        public void ImportSpecificationsService_ImportSpecifications_ShouldSetSpecifications()
        {
            var productDTO = new ProductImportDataTransferObject()
                                 {
                                     UrlSegment = "test-url",
                                     Specifications = new Dictionary<string, string>()
                                                          {
                                                              {"Storage","16GB"}
                                                          }
                                 };

            var product = new Product() { Name = "Test Product" };
            _importSpecificationsService.Initialize();

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            product.SpecificationValues.Should().HaveCount(1);
        }
    }
}