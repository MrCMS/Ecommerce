using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class CategoryContainerControllerTests
    {
        private readonly CategoryContainerController _controller;
        private readonly ICategoryService _categoryService;

        public CategoryContainerControllerTests()
        {
            _categoryService = A.Fake<ICategoryService>();
            _controller = new CategoryContainerController(_categoryService) {RequestMock = A.Fake<HttpRequestBase>()};
        }

        private CategoryContainer GetCategoryContainer()
        {
            return new CategoryContainer {Layout = new Layout()};
        }

        [Fact]
        public void CategoryContainerController_Show_ReturnsAViewResult()
        {
            var categoryContainer = GetCategoryContainer();

            var show = _controller.Show(categoryContainer);

            show.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CategoryContainerController_Show_ReturnsCategoryContainer()
        {
            var categoryContainer = GetCategoryContainer();

            var show = _controller.Show(categoryContainer);

            show.Model.Should().BeOfType<CategoryContainer>();
        }
    }
}