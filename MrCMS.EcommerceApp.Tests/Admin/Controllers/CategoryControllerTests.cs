using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class CategoryControllerTests
    {
        private ICategoryService _categoryService;
        private IDocumentService _documentService;
        private CategoryContainer _categoryContainer;
        private SiteSettings _siteSettings;

        [Fact]
        public void CategoryController_Index_ReturnsViewResult()
        {
            var categoryController = GetCategoryController();

            var index = categoryController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CategoryController_Index_CallsSearchOnTheCategoryService()
        {
            var categoryController = GetCategoryController();

            categoryController.Index("test", 1);

            A.CallTo(() => _categoryService.Search("test", 1,10)).MustHaveHappened();
        }

        [Fact]
        public void CategoryController_Index_ReturnsTheResultOfTheCallToSearchAsTheModel()
        {
            var categoryController = GetCategoryController();
            var categoryPagedList = new CategoryPagedList(new StaticPagedList<Category>(new Category[0], 1, 1, 0), 1);
            A.CallTo(() => _categoryService.Search("test", 1,10)).Returns(categoryPagedList);

            var index = categoryController.Index("test", 1);

            index.Model.Should().Be(categoryPagedList);
        }

        [Fact]
        public void CategoryController_Index_ReturnsNullIfTheCategoryContainerIsNull()
        {
            
            var categoryController = GetCategoryController();
            A.CallTo(() => _documentService.GetUniquePage<CategoryContainer>()).Returns(null);

            var index = categoryController.Index("test", 1);

            index.Model.Should().BeNull();
        }

        CategoryController GetCategoryController()
        {
            _documentService = A.Fake<IDocumentService>();
            _categoryContainer = new CategoryContainer();
            A.CallTo(() => _documentService.GetUniquePage<CategoryContainer>()).Returns(_categoryContainer);
            _categoryService = A.Fake<ICategoryService>();
            _siteSettings = new SiteSettings() { DefaultPageSize = 10 };
            return new CategoryController(_categoryService, _documentService, _siteSettings);
        } 
    }
}