using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ProductServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;
        private readonly ProductService _productService;
        private readonly SiteSettings _siteSettings;
        private readonly IUniquePageService _uniquePageService;

        public ProductServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _siteSettings = new SiteSettings() { DefaultPageSize = 10 };
            _uniquePageService = A.Fake<IUniquePageService>();
            _productService = new ProductService(Session, _documentService, _siteSettings, _uniquePageService);
        }

        [Fact]
        public void ProductService_Search_WithNoSearchTermAndPageReturnsTheFirstPageOfAllProducts()
        {
            var products = Enumerable.Range(1, 20).Select(i => new Product { Name = "Product " + i }).ToList();
            Session.Transact(session => products.ForEach(product => session.Save(product)));

            var pagedList = _productService.Search();

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(products.Take(10));
        }

        [Fact]
        public void ProductService_Search_WithNoSearchTermAndPageSetReturnsThatPage()
        {
            var products = Enumerable.Range(1, 20).Select(i => new Product { Name = "Product " + i }).ToList();
            Session.Transact(session => products.ForEach(product => session.Save(product)));

            var pagedList = _productService.Search(page: 2);

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(products.Skip(10).Take(10));
        }

        [Fact]
        public void ProductService_Search_WithSearchTermFiltersByThatValue()
        {
            var products1 = Enumerable.Range(1, 5).Select(i => new Product { Name = "Product " + i }).ToList();
            var products2 = Enumerable.Range(1, 5).Select(i => new Product { Name = "Other " + i }).ToList();
            Session.Transact(session => products1.ForEach(product => session.Save(product)));
            Session.Transact(session => products2.ForEach(product => session.Save(product)));

            var pagedList = _productService.Search("Other");

            pagedList.Should().HaveCount(5);
            pagedList.ShouldBeEquivalentTo(products2);
        }

        [Fact]
        public void ProductService_Search_WithSearchTermAndPageFiltersByThatValueAndPages()
        {
            var products1 = Enumerable.Range(1, 20).Select(i => new Product { Name = "Product " + i }).ToList();
            var products2 = Enumerable.Range(1, 20).Select(i => new Product { Name = "Other " + i }).ToList();
            Session.Transact(session => products1.ForEach(product => session.Save(product)));
            Session.Transact(session => products2.ForEach(product => session.Save(product)));

            var pagedList = _productService.Search("Other", 2);

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(products2.Skip(10).Take(10));
        }
        [Fact]
        public void ProductService_Search_ReturnsTheIdOfTheProductContainerIfItExists()
        {
            A.CallTo(() => _uniquePageService.GetUniquePage<ProductContainer>()).Returns(new ProductContainer { Id = 1 });

            var pagedList = _productService.Search();

            pagedList.ProductContainerId.Should().Be(1);
        }

        [Fact]
        public void ProductService_Search_ReturnsNullContainerIdIfItDoesNotExist()
        {
            A.CallTo(() => _uniquePageService.GetUniquePage<ProductContainer>()).Returns(null);

            var pagedList = _productService.Search();

            pagedList.ProductContainerId.Should().Be(null);
        }
    }
}