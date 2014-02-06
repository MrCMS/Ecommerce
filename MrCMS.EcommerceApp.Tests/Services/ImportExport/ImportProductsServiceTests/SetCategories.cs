using System.Data.Common;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class SetCategories
    {
        [Fact]
        public void IfDTOHasNewCategoryWhichExistsInSystemItIsAddedToTheProduct()
        {
            var category = new CategoryBuilder().WithUrlSegment("test-category").Build();
            var importProductsService = new ImportProductsServiceBuilder().WithExistingDocuments(category).Build();
            var dataTransferObject = new ProductImportDTOBuilder().WithCategories("test-category").Build();
            var product = new ProductBuilder().Build();
            product.Categories.Where(c => c.UrlSegment == "test-category").Should().HaveCount(0);

            importProductsService.SetCategories(dataTransferObject, product);

            product.Categories.Where(c => c.UrlSegment == "test-category").Should().HaveCount(1);
        }

        [Fact]
        public void IfDTOHasNoCategoriesThenNoCategoriesAreAdded()
        {
            var category = new CategoryBuilder().WithUrlSegment("test-category").Build();
            var importProductsService = new ImportProductsServiceBuilder().WithExistingDocuments(category).Build();
            var dataTransferObject = new ProductImportDTOBuilder().WithNoCategories().Build();
            var product = new ProductBuilder().Build();
            product.Categories.Where(c => c.UrlSegment == "test-category").Should().HaveCount(0);

            importProductsService.SetCategories(dataTransferObject, product);

            product.Categories.Where(c => c.UrlSegment == "test-category").Should().HaveCount(0);
        }

        [Fact]
        public void IfDTOHasNoCategoriesAndOneExistsOnTheProductRemoveCategoryFromProduct()
        {
            var category = new CategoryBuilder().WithUrlSegment("test-category").Build();
            var importProductsService = new ImportProductsServiceBuilder().WithExistingDocuments(category).Build();
            var dataTransferObject = new ProductImportDTOBuilder().WithNoCategories().Build();
            var product = new ProductBuilder().WithCategories(category).Build();
            product.Categories.Where(c => c.UrlSegment == "test-category").Should().HaveCount(1);

            importProductsService.SetCategories(dataTransferObject, product);

            product.Categories.Where(c => c.UrlSegment == "test-category").Should().HaveCount(0);
        }

        [Fact]
        public void IfDTOHasNoCategoriesAndOneExistsOnTheProductRemoveProductFromCategory()
        {
            var category = new CategoryBuilder().WithUrlSegment("test-category").Build();
            var importProductsService = new ImportProductsServiceBuilder().WithExistingDocuments(category).Build();
            var dataTransferObject = new ProductImportDTOBuilder().WithNoCategories().Build();
            var product = new ProductBuilder().WithCategories(category).Build();
            category.Products.Add(product);

            importProductsService.SetCategories(dataTransferObject, product);

            category.Products.Should().NotContain(product);
        }
    }
}