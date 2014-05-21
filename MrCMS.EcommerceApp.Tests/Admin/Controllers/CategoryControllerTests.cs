using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class CategoryControllerTests
    {
        private readonly ICategoryAdminService _categoryAdminService;
        private readonly CategoryController _categoryController;

        public CategoryControllerTests()
        {
            _categoryAdminService = A.Fake<ICategoryAdminService>();
            //use the default of the container existing
            A.CallTo(() => _categoryAdminService.CategoryContainerExists()).Returns(true);
            _categoryController = new CategoryController(_categoryAdminService);
        }

        [Fact]
        public void CategoryController_Index_ReturnsViewResult()
        {
            ViewResult index = _categoryController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CategoryController_Index_CallsSearchOnTheCategoryService()
        {
            _categoryController.Index("test", 1);

            A.CallTo(() => _categoryAdminService.Search("test", 1)).MustHaveHappened();
        }

        [Fact]
        public void CategoryController_Index_ReturnsTheResultOfTheCallToSearchAsTheModel()
        {
            var categoryPagedList = new CategoryPagedList(new StaticPagedList<Category>(new Category[0], 1, 1, 0), 1);
            A.CallTo(() => _categoryAdminService.Search("test", 1)).Returns(categoryPagedList);

            ViewResult index = _categoryController.Index("test", 1);

            index.Model.Should().Be(categoryPagedList);
        }

        [Fact]
        public void CategoryController_Index_ModelIsNullIfTheCategoryContainerDoesNotExist()
        {
            A.CallTo(() => _categoryAdminService.CategoryContainerExists()).Returns(false);

            ViewResult index = _categoryController.Index("test", 1);

            index.Model.Should().BeNull();
        }
    }
}