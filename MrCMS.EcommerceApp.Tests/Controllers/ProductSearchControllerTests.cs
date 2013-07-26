using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class ProductSearchControllerTests
    {
        private readonly ICategoryService _categoryService;
        private readonly ProductSearchController _controller;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IProductService _productService;
        private readonly IProductSearchService _productSearchService;
        private readonly IBrandService _brandService;

        public ProductSearchControllerTests()
        {
            _productOptionManager = A.Fake<IProductOptionManager>();
            _productService = A.Fake<IProductService>();
            _categoryService = A.Fake<ICategoryService>();
            _productSearchService = A.Fake<IProductSearchService>();
            _brandService = A.Fake<IBrandService>();
            _controller = new ProductSearchController(_categoryService, _productOptionManager, _productSearchService, _brandService) { RequestMock = A.Fake<HttpRequestBase>() };
        }

        private ProductSearch GetProductSearch()
        {
            return new ProductSearch { Layout = new Layout() };
        }

        [Fact]
        public void ProductSearchController_Show_ReturnsAViewResult()
        {
            var productSearch = GetProductSearch();

            var show = _controller.Show(productSearch, new ProductSearchQuery());

            show.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductSearchController_Show_ReturnsProductSearch()
        {
            var productSearch = GetProductSearch();

            var show = _controller.Show(productSearch, new ProductSearchQuery());

            show.Model.Should().BeOfType<ProductSearch>();
        }
    }
}