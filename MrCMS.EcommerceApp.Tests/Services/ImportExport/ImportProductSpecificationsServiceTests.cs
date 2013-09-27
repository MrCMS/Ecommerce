using System.Collections.Generic;
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
    public class ImportProductSpecificationsServiceTests
    {
        private readonly IProductOptionManager _productOptionManager;
        private readonly ImportProductSpecificationsService _importSpecificationsService;
        private readonly IProductVariantService _productVariantService;

        public ImportProductSpecificationsServiceTests()
        {
            _productOptionManager = A.Fake<IProductOptionManager>();
            _productVariantService = A.Fake<IProductVariantService>();
            _importSpecificationsService = new ImportProductSpecificationsService(_productOptionManager, _productVariantService);
        }


        [Fact]
        public void ImportSpecificationsService_ImportVariantSpecifications_ShouldAddOptionsToProduct()
        {
            var productVariantDTO = new ProductVariantImportDataTransferObject
                                        {
                                            SKU = "123",
                                            Options = new Dictionary<string, string>() { { "Storage", "16GB" } }
                                        };
            var productDTO = new ProductImportDataTransferObject
                                 {
                                     UrlSegment = "test-url",
                                     ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
                                 };

            var product = new Product() { Name = "Test Product" };
            var productVariant = new ProductVariant() { Name = "Test Product Variant", Product = product };

            var option = new ProductOption() { Id = 1, Name = "Storage" };
            A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).Returns(option);

            _importSpecificationsService.ImportVariantSpecifications(productVariantDTO, product, productVariant);

            product.Options.Should().HaveCount(1);
        }

        [Fact(Skip = "Refactoring")]
        public void ImportSpecificationsService_ImportSpecifications_ShouldAddANewSpecificationAttributeOptionIfItDoesntExist()
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

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            A.CallTo(() => _productOptionManager.UpdateSpecificationAttribute(A<ProductSpecificationAttribute>.Ignored)).MustHaveHappened();
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

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            A.CallTo(() => _productOptionManager.AddSpecificationAttribute(A<ProductSpecificationAttribute>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void ImportSpecificationsService_ImportSpecifications_ShouldCallGetAttributeOptionByNameOfProductOptionManager()
        {
            var productVariantDTO = new ProductVariantImportDataTransferObject()
                                        {
                                            SKU = "123",
                                            Options = new Dictionary<string, string>() { { "Storage", "16GB" } }
                                        };
            var productDTO = new ProductImportDataTransferObject()
                                 {
                                     UrlSegment = "test-url",
                                     ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
                                 };

            var product = new Product();
            var productVariant = new ProductVariant { Product = product };
            _importSpecificationsService.ImportVariantSpecifications(productVariantDTO, product, productVariant);

            A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).MustHaveHappened();
        }

        [Fact]
        public void ImportSpecificationsService_ImportSpecifications_ShouldCallAnyExistingSpecificationAttributesWithNameOfProductOptionManager()
        {
            var productDTO = new ProductImportDataTransferObject()
                                 {
                                     UrlSegment = "test-url",
                                     Specifications = new Dictionary<string, string>() { { "Storage", "16GB" } }
                                 };

            var product = new Product();
            _importSpecificationsService.ImportSpecifications(productDTO, product);

            A.CallTo(() => _productOptionManager.AnyExistingSpecificationAttributesWithName("Storage")).MustHaveHappened();
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
            _importSpecificationsService.ImportSpecifications(productDTO, product);

            A.CallTo(() => _productOptionManager.GetSpecificationAttributeByName("Storage")).MustHaveHappened();
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

            _importSpecificationsService.ImportSpecifications(productDTO, product);

            product.SpecificationValues.Should().HaveCount(1);
        }
    }
}