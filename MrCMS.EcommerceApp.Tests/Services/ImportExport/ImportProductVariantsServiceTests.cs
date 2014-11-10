using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using NHibernate;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportProductVariantsServiceTests
    {
        private readonly IImportProductVariantPriceBreaksService _importProductVariantPriceBreaksService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly ImportProductVariantsService _importProductVariantsService;

        public ImportProductVariantsServiceTests()
        {
            _importProductVariantPriceBreaksService = A.Fake<IImportProductVariantPriceBreaksService>();
            _taxRateManager = A.Fake<ITaxRateManager>();
            _importProductVariantsService = new ImportProductVariantsService(_importProductVariantPriceBreaksService,
                                                                              _taxRateManager,  A.Fake<ISession>());
        }

        [Fact(Skip = "To be refactored")]
        public void ImportProductVariantsService_ImportVariants_ShouldSetProductVariantTaxRate()
        {
            var productVariantDTO = new ProductVariantImportDataTransferObject
                                        {
                                            SKU = "123",
                                            TaxRate = 1
                                        };
            var productDTO = new ProductImportDataTransferObject
                                 {
                                     UrlSegment = "test-url",
                                     ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
                                 };

            var product = new Product { Name = "Test Product" };

            var taxRate = new TaxRate() { Id = 1, Name = "GLOBAL" };
            A.CallTo(() => _taxRateManager.Get(productVariantDTO.TaxRate.Value)).Returns(taxRate);

            var result = _importProductVariantsService.ImportVariants(productDTO, product);

            result.First().TaxRate.Name.ShouldBeEquivalentTo("GLOBAL");
        }

        [Fact(Skip = "To be refactored")]
        public void ImportProductVariantsService_ImportVariants_ShouldSetProductVariantPrimaryProperties()
        {
            var productVariantDTO = new ProductVariantImportDataTransferObject
                                        {
                                            PreviousPrice = 2,
                                            Price = 1,
                                            SKU = "123",
                                            Name = "Test Product Variant",
                                            TrackingPolicy = TrackingPolicy.Track,
                                            Weight = 0,
                                            Barcode = "456",
                                            Stock = 5
                                        };
            var productDTO = new ProductImportDataTransferObject
                                 {
                                     UrlSegment = "test-url",
                                     ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
                                 };

            var product = new Product() { Name = "Test Product" };

            var result = _importProductVariantsService.ImportVariants(productDTO, product);

            result.First().PreviousPrice.ShouldBeEquivalentTo(2);
            result.First().BasePrice.ShouldBeEquivalentTo(1);
            result.First().SKU.ShouldBeEquivalentTo("123");
            result.First().Name.ShouldBeEquivalentTo("Test Product Variant");
            result.First().TrackingPolicy.ShouldBeEquivalentTo(TrackingPolicy.Track);
            result.First().Weight.ShouldBeEquivalentTo(0);
            result.First().Barcode.ShouldBeEquivalentTo("456");
            result.First().StockRemaining.ShouldBeEquivalentTo(5);
        }


        [Fact(Skip = "To be refactored")]
        public void ImportProductVariantsService_ImportVariants_ShouldAddVariantsToProduct()
        {
            var productVariantDTO = new ProductVariantImportDataTransferObject
                                        {
                                            SKU = "123"
                                        };
            var productDTO = new ProductImportDataTransferObject
                                 {
                                     UrlSegment = "test-url",
                                     ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
                                 };

            var product = new Product() { Name = "Test Product" };

            _importProductVariantsService.ImportVariants(productDTO, product);

            product.Variants.Should().HaveCount(1);
        }

        [Fact(Skip = "To be refactored")]
        public void ImportProductVariantsService_ImportVariants_ShouldCallGetProductVariantBySKUOfProductVariantService()
        {
            var productVariant = new ProductVariantImportDataTransferObject
                                     {
                                         SKU = "123"
                                     };
            var productDTO = new ProductImportDataTransferObject
                                 {
                                     UrlSegment = "test-url",
                                     ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariant }
                                 };

            var product = new Product();
            _importProductVariantsService.ImportVariants(productDTO, product);
        }

        [Fact(Skip = "To be refactored")]
        public void ImportProductVariantsService_ImportVariants_ShouldCallImportVariantPriceBreaksOfImportProductVariantPriceBreaksService()
        {
            var productVariantDTO = new ProductVariantImportDataTransferObject
            {
                SKU = "123",
                PriceBreaks = new Dictionary<int, decimal>() { { 10, 299 } }
            };
            var productDTO = new ProductImportDataTransferObject
            {
                UrlSegment = "test-url",
                ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
            };

            var product = new Product();

            var productVariant = new ProductVariant() { Name = "Test Product Variant" };

            _importProductVariantsService.ImportVariants(productDTO, product);

            A.CallTo(() => _importProductVariantPriceBreaksService.ImportVariantPriceBreaks(productVariantDTO, productVariant)).MustHaveHappened();
        }
    }
}