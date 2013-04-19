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
    public class ProductContainerControllerTests
    {
        private readonly ProductContainerController _controller;

        public ProductContainerControllerTests()
        {
            _controller = new ProductContainerController {RequestMock = A.Fake<HttpRequestBase>()};
        }

        private ProductSearch GetProductContainer()
        {
            return new ProductSearch {Layout = new Layout()};
        }

        [Fact]
        public void ProductContainerController_Show_ReturnsAViewResult()
        {
            var productContainer = GetProductContainer();

            var show = _controller.Show(productContainer);

            show.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductContainerController_Show_ReturnsProductContainer()
        {
            var productContainer = GetProductContainer();

            var show = _controller.Show(productContainer);

            show.Model.Should().BeOfType<ProductSearch>();
        }
    }
}