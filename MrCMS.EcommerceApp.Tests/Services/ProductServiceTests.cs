using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ProductServiceTests : InMemoryDatabaseTest
    {
        private IDocumentService _documentService;

        [Fact]
        public void ProductService_Search_WithNoSearchTermAndPageReturnsTheFirstPageOfAllProducts()
        {
            var productService = GetProductService();
            var products = Enumerable.Range(1, 20).Select(i => new Product { Name = "Product " + i }).ToList();
            Session.Transact(session => products.ForEach(product => session.Save(product)));

            var pagedList = productService.Search();

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(products.Take(10));
        }

        [Fact]
        public void ProductService_Search_WithNoSearchTermAndPageSetReturnsThatPage()
        {
            var productService = GetProductService();
            var products = Enumerable.Range(1, 20).Select(i => new Product { Name = "Product " + i }).ToList();
            Session.Transact(session => products.ForEach(product => session.Save(product)));

            var pagedList = productService.Search(page: 2);

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(products.Skip(10).Take(10));
        }

        [Fact]
        public void ProductService_Search_WithSearchTermFiltersByThatValue()
        {
            var productService = GetProductService();
            var products1 = Enumerable.Range(1, 5).Select(i => new Product { Name = "Product " + i }).ToList();
            var products2 = Enumerable.Range(1, 5).Select(i => new Product { Name = "Other " + i }).ToList();
            Session.Transact(session => products1.ForEach(product => session.Save(product)));
            Session.Transact(session => products2.ForEach(product => session.Save(product)));

            var pagedList = productService.Search("Other");

            pagedList.Should().HaveCount(5);
            pagedList.ShouldBeEquivalentTo(products2);
        }

        [Fact]
        public void ProductService_Search_WithSearchTermAndPageFiltersByThatValueAndPages()
        {
            var productService = GetProductService();
            var products1 = Enumerable.Range(1, 20).Select(i => new Product { Name = "Product " + i }).ToList();
            var products2 = Enumerable.Range(1, 20).Select(i => new Product { Name = "Other " + i }).ToList();
            Session.Transact(session => products1.ForEach(product => session.Save(product)));
            Session.Transact(session => products2.ForEach(product => session.Save(product)));

            var pagedList = productService.Search("Other", 2);

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(products2.Skip(10).Take(10));
        }
        [Fact]
        public void ProductService_Search_ReturnsTheIdOfTheProductContainerIfItExists()
        {
            var productService = GetProductService();
            A.CallTo(() => _documentService.GetUniquePage<ProductContainer>()).Returns(new ProductContainer {Id = 1});

            var pagedList = productService.Search();

            pagedList.ProductContainerId.Should().Be(1);
        }

        [Fact]
        public void ProductService_Search_ReturnsNullContainerIdIfItDoesNotExist()
        {
            var productService = GetProductService();
            A.CallTo(() => _documentService.GetUniquePage<ProductContainer>()).Returns(null);

            var pagedList = productService.Search();

            pagedList.ProductContainerId.Should().Be(null);
        }

        ProductService GetProductService()
        {
            _documentService = A.Fake<IDocumentService>();
            return new ProductService(Session, _documentService);
        }
    }
}