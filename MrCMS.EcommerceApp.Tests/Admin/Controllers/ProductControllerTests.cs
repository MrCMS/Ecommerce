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
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductControllerTests
    {
        private IProductService _productService;
        private IDocumentService _documentService;
        private ProductContainer _productContainer;
        private ProductController _productController;
        private ICategoryService _categoryService;

        public ProductControllerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _productContainer = new ProductContainer();
            A.CallTo(() => _documentService.GetUniquePage<ProductContainer>()).Returns(_productContainer);
            _productService = A.Fake<IProductService>();
            _categoryService = A.Fake<ICategoryService>();
            _productController = new ProductController(_productService, _documentService, _categoryService);
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
            A.CallTo(() => _documentService.GetUniquePage<ProductContainer>()).Returns(null);

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

            result.Model.Should().Be(product);
        }

        [Fact]
        public void ProductController_MakeMultiVariantPost_ShouldCallTheMakeMultiVariantMethodOfTheServiceWithThePassedArguments()
        {
            var product = new Product();

            var result = _productController.MakeMultiVariant(product, "option1", "option2", "option3");

            A.CallTo(() => _productService.MakeMultiVariant(product, "option1", "option2", "option3"))
             .MustHaveHappened();
        }

        [Fact]
        public void ProductController_MakeMultiVariantPost_ShouldReturnARedirectToRouteResult()
        {
            var product = new Product();

            var result = _productController.MakeMultiVariant(product, "option1", "option2", "option3");

            result.Should().BeOfType<RedirectToRouteResult>();
        }
        [Fact]
        public void ProductController_MakeMultiVariantPost_ShouldRedirectToTheWebpageEditActionForTheProductId()
        {
            var product = new Product{Id=123};

            var result = _productController.MakeMultiVariant(product, "option1", "option2", "option3");

            result.RouteValues["controller"].Should().Be("Webpage");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }
    }
}