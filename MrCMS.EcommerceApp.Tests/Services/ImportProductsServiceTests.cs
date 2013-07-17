using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportProductsServiceTests
    {
        private readonly IDocumentService _documentService;
        private readonly IBrandService _brandService;
        private readonly ImportProductsService _importProductsService;
        private readonly IImportSpecificationsService _importSpecificationsService;
        private readonly IImportProductVariantsService _importProductVariantsService;
        private readonly IImportProductImagesService _importProductImagesService;

        public ImportProductsServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _brandService = A.Fake<IBrandService>();
            _importSpecificationsService = A.Fake<IImportSpecificationsService>();
            _importProductVariantsService = A.Fake<IImportProductVariantsService>();
            _importProductImagesService = A.Fake<IImportProductImagesService>();
            _importProductsService = new ImportProductsService(_documentService, _brandService,
                                                               _importSpecificationsService,
                                                               _importProductVariantsService,
                                                               _importProductImagesService);
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
        public void ImportProductsService_ImportProduct_ShouldCallGetDocumentOfDocumentService()
        {
            var product = new ProductImportDataTransferObject
                              {
                                  UrlSegment = "test-url",
                                  Categories = new List<int> { 1 }
                              };

            _importProductsService.ImportProduct(product);

            A.CallTo(() => _documentService.GetDocument<Category>(1)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldSetProductPrimaryProperties()
        {
            var product = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Name = "Test Product",
                Abstract = "Test Abstract",
                Description = "Test Description",
                SEODescription = "Test SEO Description",
                SEOKeywords = "Test, Thought",
                SEOTitle = "Test SEO Title"
            };

            var result = _importProductsService.ImportProduct(product);

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

            var brand = new Brand { Name = "Test Brand" };
            A.CallTo(() => _brandService.GetBrandByName("Test Brand")).Returns(brand);

            var importProduct = _importProductsService.ImportProduct(product);

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

            A.CallTo(() => _brandService.GetBrandByName("Test Brand")).Returns(null);

            var importProduct = _importProductsService.ImportProduct(product);

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

            var importProduct = _importProductsService.ImportProduct(product);

            importProduct.Brand.Name.Should().Be(brand.Name);
        }

        [Fact]
        public void ImportProductsService_ImportProducts_ShouldSetCategoriesIfTheyExist()
        {
            var productDTO = new ProductImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Categories = new List<int>() { 1 }
            };

            var category = new Category() { Id = 1, Name = "Test Category" };
            A.CallTo(() => _documentService.GetDocument<Category>(1)).Returns(category);

            var product = new Product() { Name = "Test Product" };
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var importProduct = _importProductsService.ImportProduct(productDTO);

            importProduct.Categories.Should().HaveCount(1);
        }
    }
}