using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class ProductSearchControllerTests
    {
        private readonly ProductSearchController _controller;

        public ProductSearchControllerTests()
        {
            _controller = new ProductSearchController {RequestMock = A.Fake<HttpRequestBase>()};
        }

        private ProductSearch GetProductSearch()
        {
            return new ProductSearch {Layout = new Layout()};
        }

        [Fact]
        public void ProductSearchController_Show_ReturnsAViewResult()
        {
            var productSearch = GetProductSearch();

            var show = _controller.Show(productSearch);

            show.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductSearchController_Show_ReturnsProductSearch()
        {
            var productSearch = GetProductSearch();

            var show = _controller.Show(productSearch);

            show.Model.Should().BeOfType<ProductSearch>();
        }
    }
}