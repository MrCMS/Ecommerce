using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.ActionResults;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductControllerTests
    {
        private IProductService _productService;
        private IDocumentService _documentService;
        private ProductSearch _productSearch;
        private ProductController _productController;
        private ICategoryService _categoryService;
        private readonly ITaxRateManager _taxRateManager;
        private IProductOptionManager _productOptionManager;
        private readonly IFileService _fileService;

        public ProductControllerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _productSearch = new ProductSearch();
            A.CallTo(() => _documentService.GetUniquePage<ProductSearch>()).Returns(_productSearch);
            _productService = A.Fake<IProductService>();
            _categoryService = A.Fake<ICategoryService>();
            _productOptionManager = A.Fake<IProductOptionManager>();
            _fileService = A.Fake<IFileService>();
            _productController = new ProductController(_productService, _documentService, _categoryService, _taxRateManager, _productOptionManager,_fileService);
        }

        [Fact]
        public void ProductController_Index_ShouldReturnAViewResult()
        {
            var index = _productController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductController_Index_ShouldCallProductServiceSearch()
        {
            _productController.Index("q", 1);

            A.CallTo(() => _productService.Search("q", 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_Index_ShouldReturnTheResultOfTheProductSearchCallAsTheModel()
        {
            var pagedList = new ProductPagedList(new StaticPagedList<Product>(new List<Product>(), 1, 1, 0), 1);
            A.CallTo(() => _productService.Search("q", 1)).Returns(pagedList);

            var viewResult = _productController.Index("q", 1);

            viewResult.Model.Should().Be(pagedList);
        }

        [Fact]
        public void ProductController_Index_ShouldReturnNullModelIfProductContainerIsNull()
        {
            A.CallTo(() => _documentService.GetUniquePage<ProductSearch>()).Returns(null);

            var viewResult = _productController.Index();

            viewResult.Model.Should().BeNull();
        }

        [Fact]
        public void ProductController_MakeMultiVariantGet_ShouldReturnAPartialViewResult()
        {
            var product = new Product();

            var result = _productController.MakeMultiVariant(product);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductController_MakeMultivariantGet_ReturnsThePassedObjectAsTheModel()
        {
            var product = new Product();

            var result = _productController.MakeMultiVariant(product);

            result.Model.Should().BeOfType<MakeMultivariantModel>();
        }

        [Fact]
        public void ProductController_MakeMultiVariantPost_ShouldCallTheMakeMultiVariantMethodOfTheServiceWithThePassedArguments()
        {
            var multiVariantModel = new MakeMultivariantModel();

            var result = _productController.MakeMultiVariant(multiVariantModel);

            A.CallTo(() => _productService.MakeMultiVariant(multiVariantModel))
             .MustHaveHappened();
        }

        [Fact]
        public void ProductController_MakeMultiVariantPost_ShouldReturnARedirectToRouteResult()
        {
            var multiVariantModel = new MakeMultivariantModel();

            var result = _productController.MakeMultiVariant(multiVariantModel);

            result.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductController_MakeMultiVariantPost_ShouldRedirectToTheWebpageEditActionForTheProductId()
        {
            var multiVariantModel = new MakeMultivariantModel
                {
                    ProductId = 123
                };

            var result = _productController.MakeMultiVariant(multiVariantModel);

            result.RouteValues["controller"].Should().Be("Webpage");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        //[Fact]
        //public void ProductController_ViewCategories_ShouldReturnAPartialViewResult()
        //{
        //    var product = new Product { Id = 123 };

        //    var result = _productController.ViewCategories(product);

        //    result.Should().BeOfType<PartialViewResult>();
        //}

        //[Fact]
        //public void ProductController_ViewCategories_ShouldReturnThePassedProductAsTheModel()
        //{
        //    var product = new Product { Id = 123 };

        //    var result = _productController.ViewCategories(product);

        //    result.Model.Should().Be(product);
        //}

        [Fact]
        public void ProductController_AddCategory_ShouldReturnAPartialViewResult()
        {
            var product = new Product { Id = 123 };

            var result = _productController.AddCategory(product, "query", 1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductController_AddCategory_ShouldSetTheProductToTheViewData()
        {
            var product = new Product { Id = 123 };

            var result = _productController.AddCategory(product, "query", 1);

            _productController.ViewData["product"].Should().Be(product);
        }

        [Fact]
        public void ProductController_AddCategory_ShouldCallGetCategoriesOnTheCategoryService()
        {
            var product = new Product { Id = 123 };

            var result = _productController.AddCategory(product, "query", 1);

            A.CallTo(() => _categoryService.GetCategories(product, "query", 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_AddCategory_ShouldReturnTheResultOfGetCategoriesAsTheModel()
        {
            var product = new Product { Id = 123 };
            var pagedList = A.Fake<IPagedList<Category>>();
            A.CallTo(() => _categoryService.GetCategories(product, "query", 1)).Returns(pagedList);

            var result = _productController.AddCategory(product, "query", 1);

            result.Model.Should().Be(pagedList);
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ShouldReturnAJsonResult()
        {
            var product = new Product { Id = 123 };

            var result = _productController.AddCategory(product, 1);

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ShouldCallProductServiceAddCategory()
        {
            var product = new Product { Id = 123 };

            var result = _productController.AddCategory(product, 1);

            A.CallTo(() => _productService.AddCategory(product, 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ReturnAModelOfTrueForAStandardCall()
        {
            var product = new Product { Id = 123 };

            var result = _productController.AddCategory(product, 1);

            result.Data.Should().Be(true);
        }

        [Fact]
        public void ProductController_AddCategoryPOST_ReturnAModelOfFalseIfTheCallFails()
        {
            var product = new Product { Id = 123 };
            A.CallTo(() => _productService.AddCategory(product, 1)).Throws<Exception>();

            var result = _productController.AddCategory(product, 1);

            result.Data.Should().Be(false);
        }

        [Fact]
        public void ProductController_RemoveCategory_ReturnsAPartialViewResult()
        {
            var product = new Product { Id = 123 };

            var result = _productController.RemoveCategory(product, 1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductController_RemoveCategory_ReturnsPassedProductAsModel()
        {
            var product = new Product { Id = 123 };

            var result = _productController.RemoveCategory(product, 1);

            result.Model.Should().Be(product);
        }

        [Fact]
        public void ProductController_RemoveCategory_SetsViewDataForCategory()
        {
            var product = new Product { Id = 123 };
            var category = new Category();
            A.CallTo(() => _documentService.GetDocument<Category>(1)).Returns(category);

            var result = _productController.RemoveCategory(product, 1);

            _productController.ViewData["category"].Should().Be(category);
        }

        [Fact]
        public void ProductController_RemoveCategoryPOST_ShouldCallRemoveCategoryOnProductService()
        {
            var product = new Product { Id = 123 };

            _productController.RemoveCategory_POST(product, 1);

            A.CallTo(() => _productService.RemoveCategory(product, 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_RemoveCategoryPOST_ShouldRedirectToWebpageEdit()
        {
            var product = new Product { Id = 123 };

            var result = _productController.RemoveCategory_POST(product, 1);

            result.RouteValues["controller"].Should().Be("Webpage");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }
    }
}