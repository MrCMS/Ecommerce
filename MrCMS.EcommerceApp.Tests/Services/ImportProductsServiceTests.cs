using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;

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
        private readonly ICategoryService _categoryService;

        public ImportProductsServiceTests()
        {
            _productService = A.Fake<IProductService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _documentService = A.Fake<IDocumentService>();
            _productOptionManager = A.Fake<IProductOptionManager>();
            _brandService = A.Fake<IBrandService>();
            _taxRateManager = A.Fake<ITaxRateManager>();
            _fileService = A.Fake<IFileService>();
            _categoryService = A.Fake<ICategoryService>();

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

            var brand = new Brand { Name = "Test Brand" };
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
                Categories = new List<int>(){1}
            };

            var category = new Category() { Id=1, Name = "Test Category" };
            A.CallTo(() => _documentService.GetDocument<Category>(1)).Returns(category);

            var product = new Product() { Name = "Test Product" };
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var importProduct = _importProductsService.ImportProduct(productDTO);

            importProduct.Categories.Should().HaveCount(1);
        }

        [Fact]
        public void ImportProductsService_ImportProducts_ShouldSetSpecifications()
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
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var importProduct = _importProductsService.ImportProduct(productDTO);

            importProduct.SpecificationValues.Should().HaveCount(1);
        }

        [Fact]
        public void ImportProductsService_ImportProducts_ShouldAddANewSpecificationAttributeIfItDoesntExist()
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
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var importProduct = _importProductsService.ImportProduct(productDTO);

            A.CallTo(() => _productOptionManager.AddSpecificationAttribute(A<ProductSpecificationAttribute>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProducts_ShouldAddANewSpecificationAttributeOptionIfItDoesntExist()
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
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var importProduct = _importProductsService.ImportProduct(productDTO);

            A.CallTo(() => _productOptionManager.UpdateSpecificationAttribute(A<ProductSpecificationAttribute>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldAddVariantsToProduct()
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
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var productVariant = new ProductVariant() { Name = "Test Product Variant", SKU="123" };
            A.CallTo(() => _productVariantService.GetProductVariantBySKU("123")).Returns(productVariant);

            var importProduct=_importProductsService.ImportProduct(productDTO);

            importProduct.Variants.Should().HaveCount(1);
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldAddOptionsToProduct()
        {
            var productVariantDTO = new ProductVariantImportDataTransferObject
            {
                SKU = "123",
                Options = new Dictionary<string, string>(){{"Storage","16GB"}}
            };
            var productDTO = new ProductImportDataTransferObject
            {
                UrlSegment = "test-url",
                ProductVariants = new List<ProductVariantImportDataTransferObject>() { productVariantDTO }
            };

            var product = new Product() { Name = "Test Product" };
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var productVariant = new ProductVariant() { Name = "Test Product Variant", SKU = "123" };
            A.CallTo(() => _productVariantService.GetProductVariantBySKU("123")).Returns(productVariant);

            var option = new ProductAttributeOption() { Id = 1, Name = "Storage" };
            A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).Returns(option);

            var importProduct = _importProductsService.ImportProduct(productDTO);

            importProduct.AttributeOptions.Should().HaveCount(1);
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldAddOptionValuesToProductVariant()
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
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var productVariant = new ProductVariant() { Name = "Test Product Variant", SKU = "123" };
            A.CallTo(() => _productVariantService.GetProductVariantBySKU("123")).Returns(productVariant);

            var option = new ProductAttributeOption() { Id = 1, Name = "Storage" };
            A.CallTo(() => _productOptionManager.GetAttributeOptionByName("Storage")).Returns(option);

            var importProduct = _importProductsService.ImportProduct(productDTO);

            importProduct.Variants.First().AttributeValues.Should().HaveCount(1);
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldSetProductVariantPrimaryProperties()
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
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var productVariant = new ProductVariant() { Name = "Test Product Variant", SKU = "123" };
            A.CallTo(() => _productVariantService.GetProductVariantBySKU("123")).Returns(productVariant);

            var result = _importProductsService.ImportProduct(productDTO);

            result.Variants.First().PreviousPrice.ShouldBeEquivalentTo(2);
            result.Variants.First().BasePrice.ShouldBeEquivalentTo(1);
            result.Variants.First().SKU.ShouldBeEquivalentTo("123");
            result.Variants.First().Name.ShouldBeEquivalentTo("Test Product Variant");
            result.Variants.First().TrackingPolicy.ShouldBeEquivalentTo(TrackingPolicy.Track);
            result.Variants.First().Weight.ShouldBeEquivalentTo(0);
            result.Variants.First().Barcode.ShouldBeEquivalentTo("456");
            result.Variants.First().StockRemaining.ShouldBeEquivalentTo(5);
        }

        [Fact]
        public void ImportProductsService_ImportProduct_ShouldSetProductVariantTaxRate()
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

            var product = new Product() { Name = "Test Product" };
            A.CallTo(() => _documentService.GetDocumentByUrl<Product>(productDTO.UrlSegment)).Returns(product);

            var productVariant = new ProductVariant() { Name = "Test Product Variant", SKU = "123" };
            A.CallTo(() => _productVariantService.GetProductVariantBySKU("123")).Returns(productVariant);

            var taxRate = new TaxRate() { Id = 1, Name = "GLOBAL" };
            A.CallTo(() => _taxRateManager.Get(productVariantDTO.TaxRate.Value)).Returns(taxRate);

            var result = _importProductsService.ImportProduct(productDTO);

            result.Variants.First().TaxRate.Name.ShouldBeEquivalentTo("GLOBAL");
        }
    }
}