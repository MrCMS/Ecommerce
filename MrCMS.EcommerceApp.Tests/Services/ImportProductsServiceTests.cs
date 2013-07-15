using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportProductsServiceTests 
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IDocumentService _documentService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IBrandService _brandService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IFileService _fileService;
        private readonly ImportProductsService _importProductsService;

        public ImportProductsServiceTests()
        {
            _productService = A.Fake<IProductService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _documentService = A.Fake<IDocumentService>();
            _productOptionManager = A.Fake<IProductOptionManager>();
            _brandService = A.Fake<IBrandService>();
            _taxRateManager = A.Fake<ITaxRateManager>();
            _fileService = A.Fake<IFileService>();

            _importProductsService = new ImportProductsService(_productService, _productVariantService, _documentService, _productOptionManager, _brandService,
                                                               _taxRateManager, _fileService);
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetGetDocumentByUrlOfDocumentService()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
            };

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(product.UrlSegment)).MustHaveHappened();
        }


        [Fact]
        public void ImportProductsService_ImportProduct_ShouldNotCallGetOfProductService()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
            };

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _productService.Get(0)).MustNotHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetDocumentOfDocumentService()
        {
            var product = new ProductImportDataTransferObject
                              {
                                  UrlSegment = "test-url",
                                  Categories = new List<int> {1}
                              };

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _documentService.GetDocument<Category>(1)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportImageToGallery_ShouldCallReturnTrue()
        {
            var result =
                _importProductsService.ImportImageToGallery("http://www.thought.co.uk/Content/images/logo-white.png",
                                                           null);

            result.Should().BeTrue();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetProductVariantBySKUOfProductVariantService()
        {
            var productVariant = new ProductVariantImportDataTransferObject
                                     {
                                         SKU = "123"
                                     };
            var product = new ProductImportDataTransferObject
                              {
                                  UrlSegment = "test-url",
                                  ProductVariants = new List<ProductVariantImportDataTransferObject>() {productVariant}
                              };

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(productVariant.SKU)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetAttributeOptionByNameOfProductOptionManager()
        {
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

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallAnyExistingSpecificationAttributesWithNameOfProductOptionManager()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Specifications = new Dictionary<string, string>() {{"Storage","16GB"}}
            };

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _productOptionManager.AnyExistingSpecificationAttributesWithName("Storage")).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldCallGetSpecificationAttributeByNameOfProductOptionManager()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Specifications = new Dictionary<string, string>() { { "Storage", "16GB" } }
            };

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _productOptionManager.GetSpecificationAttributeByName("Storage")).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldSetProductPrimaryProperties()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Name="Test Product",
                Abstract = "Test Abstract",
                Description = "Test Description",
                SEODescription = "Test SEO Description",
                SEOKeywords = "Test, Thought",
                SEOTitle = "Test SEO Title"
            };

            var result=_importProductsService.ImportProduct(product);

            result.UrlSegment.ShouldBeEquivalentTo("test-url");
            result.Name.ShouldBeEquivalentTo("Test Product");
            result.Abstract.ShouldBeEquivalentTo("Test Abstract");
            result.BodyContent.ShouldBeEquivalentTo("Test Description");
            result.MetaDescription.ShouldBeEquivalentTo("Test SEO Description");
            result.MetaKeywords.ShouldBeEquivalentTo("Test, Thought");
            result.MetaTitle.ShouldBeEquivalentTo("Test SEO Title");
        }



        [Fact]
        public void ImportProductsService_ImportProducts_ShouldSetProductBrandIfItAlreadyExists()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Brand = "Test Brand"
            };

            var brand = new Brand {Name = "Test Brand"};
            A.CallTo(() => _brandService.GetBrandByName("Test Brand")).Returns(brand);

            Product importProduct = _importProductsService.ImportProduct(product);

            importProduct.Brand.Should().Be(brand);
        }

        [Fact]
        public void ImportProductsService_ImportProducts_ShouldAddANewBrandIfItDoesNotExist()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Brand = "Test Brand"
            };

            var brand = new Brand { Name = "Test Brand" };
            A.CallTo(() => _brandService.GetBrandByName("Test Brand")).Returns(null);

            Product importProduct = _importProductsService.ImportProduct(product);

            A.CallTo(() => _brandService.Add(A<Brand>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProducts_ShouldSetTheBrandToOneWithTheCorrectNameIfItDoesNotExist()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Brand = "Test Brand"
            };

            var brand = new Brand { Name = "Test Brand" };
            A.CallTo(() => _brandService.GetBrandByName("Test Brand")).Returns(null);

            Product importProduct = _importProductsService.ImportProduct(product);

            importProduct.Brand.Name.Should().Be(brand.Name);
        }
    }
}