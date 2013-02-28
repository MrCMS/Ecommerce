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
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductControllerTests
    {
        private IProductService _productService;
        private IDocumentService _documentService;
        private ProductContainer _productContainer;

        [Fact]
        public void ProductController_Index_ShouldReturnAViewResult()
        {
            var productController = GetProductController();

            var index = productController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductController_Index_ShouldCallProductServiceSearch()
        {
            var productController = GetProductController();

            productController.Index("q", 1);

            A.CallTo(() => _productService.Search("q", 1)).MustHaveHappened();
        }

        [Fact]
        public void ProductController_Index_ShouldReturnTheResultOfTheProductSearchCallAsTheModel()
        {
            var productController = GetProductController();
            var pagedList = new ProductPagedList(new StaticPagedList<Product>(new List<Product>(), 1, 1, 0), 1);
            A.CallTo(() => _productService.Search("q", 1)).Returns(pagedList);

            var viewResult = productController.Index("q", 1);

            viewResult.Model.Should().Be(pagedList);
        }

        [Fact]
        public void ProductController_Index_ShouldReturnNullModelIfProductContainerIsNull()
        {
            var productController = GetProductController();
            A.CallTo(() => _documentService.GetUniquePage<ProductContainer>()).Returns(null);

            var viewResult = productController.Index();

            viewResult.Model.Should().BeNull();
        }

        ProductController GetProductController()
        {
            _documentService = A.Fake<IDocumentService>();
            _productContainer = new ProductContainer();
            A.CallTo(() => _documentService.GetUniquePage<ProductContainer>()).Returns(_productContainer);
            _productService = A.Fake<IProductService>();
            return new ProductController(_productService, _documentService);
        }
    }
}