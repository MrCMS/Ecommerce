using System.Web.Mvc;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class CategoryContainerControllerTests
    {
        private CategoryContainerController _controller;
        private CategoryContainer _categoryContainer;

        public CategoryContainerControllerTests()
        {
            _controller = new CategoryContainerController();
            _categoryContainer = new CategoryContainer();
        }
        //[Fact]
        //public void CategoryContainerController_Show_ReturnsAViewResult()
        //{
        //    var show = _controller.Show(_categoryContainer);

        //    show.Should().BeOfType<ViewResult>();
        //}

        //[Fact]
        //public void CategoryContainerController_Show_ReturnsCategoryContainerOfCategories()
        //{
        //    var show = _controller.Show(_categoryContainer);

        //    show.Model.Should().BeOfType<CategoryContainer<Category>>();
        //}
    }
}