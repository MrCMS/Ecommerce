using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class ProductContainerControllerTests
    {
        private readonly ProductContainerController _controller;
        private readonly IUniquePageService _uniquePageService;

        public ProductContainerControllerTests()
        {
            _uniquePageService = A.Fake<IUniquePageService>();
            _controller = new ProductContainerController(_uniquePageService) {RequestMock = A.Fake<HttpRequestBase>()};
        }

        [Fact]
        public void ProductContainerController_Show_ReturnsRedirectToProductSearch()
        {
            var categoryContainer = new ProductContainer();
            var redirectResult = new RedirectResult("~/search");
            A.CallTo(() => _uniquePageService.RedirectTo<ProductSearch>(null)).Returns(redirectResult);

            RedirectResult show = _controller.Show(categoryContainer);

            show.Should().Be(redirectResult);
        }
    }
}