using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductControllerTests
    {
        private IProductService _productService;

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
            var pagedList = A.Fake<IPagedList<Product>>();
            A.CallTo(() => _productService.Search("q", 1)).Returns(pagedList);

            var viewResult = productController.Index("q", 1);

            viewResult.Model.Should().Be(pagedList);
        }

        ProductController GetProductController()
        {
            _productService = A.Fake<IProductService>();
            return new ProductController(_productService);
        }
    }
}