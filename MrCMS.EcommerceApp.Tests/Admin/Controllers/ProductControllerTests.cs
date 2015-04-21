using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.ActionResults;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductControllerTests
    {
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IDocumentService _documentService;
        private readonly IFileAdminService _fileService;
        private readonly ProductController _productController;
        private readonly IProductOptionManagementService _productOptionManagementService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly ProductSearch _productSearch;
        private readonly IProductService _productService;
        private readonly SiteSettings _siteSettings;
        private readonly IUniquePageService _uniquePageService;

        public ProductControllerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _productService = A.Fake<IProductService>();
            _categoryService = A.Fake<ICategoryService>();
            _productOptionManager = A.Fake<IProductOptionManager>();
            _fileService = A.Fake<IFileAdminService>();
            _brandService = A.Fake<IBrandService>();
            _productOptionManagementService = A.Fake<IProductOptionManagementService>();
            _siteSettings = new SiteSettings {DefaultPageSize = 10};
            _uniquePageService = A.Fake<IUniquePageService>();
            _productSearch = new ProductSearch();
            A.CallTo(() => _uniquePageService.GetUniquePage<ProductSearch>()).Returns(_productSearch);
            _productController = new ProductController(_productService, _documentService, _categoryService,
                _productOptionManager,
                _fileService, _brandService, _productOptionManagementService, _siteSettings, _uniquePageService);
        }

        [Fact]
        public void ProductController_Index_ShouldReturnAViewResult()
        {
            ViewResult index = _productController.Index(A.Fake<ProductAdminSearchQuery>());

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductController_Index_ShouldCallProductServiceSearch()
        {
            var productAdminSearchQuery = new ProductAdminSearchQuery();

            _productController.Index(productAdminSearchQuery);

            A.CallTo(() => _productService.Search(productAdminSearchQuery)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_Index_ShouldReturnTheSearchQueryAsTheModel()
        {
            var productAdminSearchQuery = new ProductAdminSearchQuery();

            ViewResult viewResult = _productController.Index(productAdminSearchQuery);

            viewResult.Model.Should().Be(productAdminSearchQuery);
        }

        [Fact]
        public void ProductController_Index_ShouldReturnTheSearchResultInViewData()
        {
            var pagedList = new ProductPagedList(new StaticPagedList<Product>(new List<Product>(), 1, 1, 0), 1);
            var productAdminSearchQuery = new ProductAdminSearchQuery();
            A.CallTo(() => _productService.Search(productAdminSearchQuery)).Returns(pagedList);

            ViewResult viewResult = _productController.Index(productAdminSearchQuery);

            viewResult.ViewData["results"].Should().Be(pagedList);
        }

        [Fact]
        public void ProductController_AddCategory_ShouldReturnAPartialViewResult()
        {
            var product = new Product {Id = 123};

            PartialViewResult result = _productController.AddCategory(product, "query", 1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductController_AddCategory_ShouldSetTheProductToTheViewData()
        {
            var product = new Product {Id = 123};

            PartialViewResult result = _productController.AddCategory(product, "query", 1);

            _productController.ViewData["product"].Should().Be(product);
        }

        [Fact]
        public void ProductController_AddCategory_ShouldCallGetCategoriesOnTheCategoryService()
        {
            var product = new Product {Id = 123};

            PartialViewResult result = _productController.AddCategory(product, "query", 1);

            A.CallTo(() => _categoryService.GetCategories(product, "query", 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_AddCategory_ShouldReturnTheResultOfGetCategoriesAsTheModel()
        {
            var product = new Product {Id = 123};
            var pagedList = A.Fake<IPagedList<Category>>();
            A.CallTo(() => _categoryService.GetCategories(product, "query", 1)).Returns(pagedList);

            PartialViewResult result = _productController.AddCategory(product, "query", 1);

            result.Model.Should().Be(pagedList);
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ShouldReturnAJsonResult()
        {
            var product = new Product {Id = 123};

            var result = _productController.AddCategory(product, 1);

            result.Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ShouldCallProductServiceAddCategory()
        {
            var product = new Product {Id = 123};

            JsonResult result = _productController.AddCategory(product, 1);

            A.CallTo(() => _productService.AddCategory(product, 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ReturnAModelOfTrueForAStandardCall()
        {
            var product = new Product {Id = 123};

            JsonResult result = _productController.AddCategory(product, 1);

            result.Data.Should().Be(true);
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ReturnAModelOfFalseIfTheCallFails()
        {
            var product = new Product {Id = 123};
            A.CallTo(() => _productService.AddCategory(product, 1)).Throws<Exception>();

            JsonResult result = _productController.AddCategory(product, 1);

            result.Data.Should().Be(false);
        }

        [Fact]
        public void ProductController_RemoveCategory_ReturnsAPartialViewResult()
        {
            var product = new Product {Id = 123};

            PartialViewResult result = _productController.RemoveCategory(product, 1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductController_RemoveCategory_ReturnsPassedProductAsModel()
        {
            var product = new Product {Id = 123};

            PartialViewResult result = _productController.RemoveCategory(product, 1);

            result.Model.Should().Be(product);
        }

        [Fact]
        public void ProductController_RemoveCategory_SetsViewDataForCategory()
        {
            var product = new Product {Id = 123};
            var category = new Category();
            A.CallTo(() => _documentService.GetDocument<Category>(1)).Returns(category);

            PartialViewResult result = _productController.RemoveCategory(product, 1);

            _productController.ViewData["category"].Should().Be(category);
        }

        [Fact]
        public void ProductController_RemoveCategoryPOST_ShouldCallRemoveCategoryOnProductService()
        {
            var product = new Product {Id = 123};

            _productController.RemoveCategory_POST(product, 1);

            A.CallTo(() => _productService.RemoveCategory(product, 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_RemoveCategoryPOST_ShouldRedirectToWebpageEdit()
        {
            var product = new Product {Id = 123};

            RedirectToRouteResult result = _productController.RemoveCategory_POST(product, 1);

            result.RouteValues["controller"].Should().Be("Webpage");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }
    }
}