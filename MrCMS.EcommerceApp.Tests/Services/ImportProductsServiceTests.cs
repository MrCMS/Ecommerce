using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportProductsServiceTests : InMemoryDatabaseTest
    {
        private IProductService _productService;
        private IProductVariantService _productVariantService;
        private IDocumentService _documentService;
        private IProductOptionManager _productOptionManager;
        private IBrandService _brandService;
        private ITaxRateManager _taxRateManager;
        private IFileService _fileService;

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetGetDocumentByUrlOfDocumentService()
        {
            var importProductsService = GetImportProductsService();

            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
            };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(product.UrlSegment)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallAnyExistingBrandsWithNameOfBrandService()
        {
            var importProductsService = GetImportProductsService();

            var product = new ProductImportDataTransferObject()
            {
                Brand = "test",
            };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _brandService.AnyExistingBrandsWithName(product.Brand,0)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetOfProductService()
        {
            var importProductsService = GetImportProductsService();

            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
            };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _productService.Get(0)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetDocumentOfDocumentService()
        {
            var importProductsService = GetImportProductsService();

            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Categories = new List<int>() {1}
            };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _documentService.GetDocument<Category>(1)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportImageToGallery_ShouldCallReturnTrue()
        {
            var importProductsService = GetImportProductsService();

            var result=importProductsService.ImportImageToGallery("http://www.thought.co.uk/Content/images/logo-white.png",null);

            result.Should().BeTrue();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetProductVariantBySKUOfProductVariantService()
        {
            var importProductsService = GetImportProductsService();

            var productVariant = new ProductVariantImportDataTransferObject()
                {
                    SKU = "123"
                };
            var product = new ProductImportDataTransferObject()
                {
                    UrlSegment = "test-url",
                    ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariant }
                };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(productVariant.SKU)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetAttributeOptionByNameOfProductOptionManager()
        {
            var importProductsService = GetImportProductsService();

            var productVariant = new ProductVariantImportDataTransferObject()
            {
                SKU = "123",
                Options = new Dictionary<string, string>() { { "Storage", "16GB" } }
            };
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariant }
            };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallAnyExistingSpecificationAttributesWithNameOfProductOptionManager()
        {
            var importProductsService = GetImportProductsService();

            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Specifications = new Dictionary<string, string>() {{"Storage","16GB"}}
            };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _productOptionManager.AnyExistingSpecificationAttributesWithName("Storage")).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetSpecificationAttributeByNameOfProductOptionManager()
        {
            var importProductsService = GetImportProductsService();

            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Specifications = new Dictionary<string, string>() { { "Storage", "16GB" } }
            };

            importProductsService.ImportProduct(product);

            A.CallTo(() => _productOptionManager.GetSpecificationAttributeByName("Storage")).MustHaveHappened();
        }

        ImportProductsService GetImportProductsService()
        {
            _productService = A.Fake<IProductService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _documentService = A.Fake<IDocumentService>();
            _productOptionManager = A.Fake<IProductOptionManager>();
            _brandService = A.Fake<IBrandService>();
            _taxRateManager = A.Fake<ITaxRateManager>();
            _fileService = A.Fake<IFileService>();

            return new ImportProductsService(_productService, _productVariantService, _documentService, _productOptionManager, _brandService,
                _taxRateManager,_fileService);
        }
    }
}