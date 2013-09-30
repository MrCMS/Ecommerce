using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class CategoryServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;
        private readonly IProductSearchService _productSearchService;
        private readonly CategoryService _categoryService;
        private readonly EcommerceSettings _ecommerceSettings;

        public CategoryServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _productSearchService = A.Fake<IProductSearchService>();
            _ecommerceSettings = new EcommerceSettings();
            _categoryService = new CategoryService(Session, CurrentSite, _documentService, _productSearchService,
                                                   _ecommerceSettings);
        }

        [Fact]
        public void CategoryService_Search_WithNoSearchTermAndPageReturnsTheFirstPageOfAllCategorys()
        {
            var categorys = Enumerable.Range(1, 20).Select(i => new Category { Name = "Category " + i }).ToList();
            Session.Transact(session => categorys.ForEach(category => session.Save(category)));

            var pagedList = _categoryService.Search();

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(categorys.Take(10));
        }

        [Fact]
        public void CategoryService_Search_WithNoSearchTermAndPageSetReturnsThatPage()
        {
            var categorys = Enumerable.Range(1, 20).Select(i => new Category { Name = "Category " + i }).ToList();
            Session.Transact(session => categorys.ForEach(category => session.Save(category)));

            var pagedList = _categoryService.Search(page: 2);

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(categorys.Skip(10).Take(10));
        }

        [Fact]
        public void CategoryService_Search_WithSearchTermFiltersByThatValue()
        {
            var categorys1 = Enumerable.Range(1, 5).Select(i => new Category { Name = "Category " + i }).ToList();
            var categorys2 = Enumerable.Range(1, 5).Select(i => new Category { Name = "Other " + i }).ToList();
            Session.Transact(session => categorys1.ForEach(category => session.Save(category)));
            Session.Transact(session => categorys2.ForEach(category => session.Save(category)));

            var pagedList = _categoryService.Search("Other");

            pagedList.Should().HaveCount(5);
            pagedList.ShouldBeEquivalentTo(categorys2);
        }

        [Fact]
        public void CategoryService_Search_WithSearchTermAndPageFiltersByThatValueAndPages()
        {
            var categorys1 = Enumerable.Range(1, 20).Select(i => new Category { Name = "Category " + i }).ToList();
            var categorys2 = Enumerable.Range(1, 20).Select(i => new Category { Name = "Other " + i }).ToList();
            Session.Transact(session => categorys1.ForEach(category => session.Save(category)));
            Session.Transact(session => categorys2.ForEach(category => session.Save(category)));

            var pagedList = _categoryService.Search("Other", 2);

            pagedList.Should().HaveCount(10);
            pagedList.ShouldBeEquivalentTo(categorys2.Skip(10).Take(10));
        }
        [Fact]
        public void CategoryService_Search_ReturnsTheIdOfTheCategoryContainerIfItExists()
        {
            A.CallTo(() => _documentService.GetUniquePage<CategoryContainer>()).Returns(new CategoryContainer { Id = 1 });

            var pagedList = _categoryService.Search();

            pagedList.CategoryContainerId.Should().Be(1);
        }

        [Fact]
        public void CategoryService_Search_ReturnsNullContainerIdIfItDoesNotExist()
        {
            A.CallTo(() => _documentService.GetUniquePage<CategoryContainer>()).Returns(null);

            var pagedList = _categoryService.Search();

            pagedList.CategoryContainerId.Should().Be(null);
        }
    }
}
