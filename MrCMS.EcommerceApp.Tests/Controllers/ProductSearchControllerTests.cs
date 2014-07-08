using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class ProductSearchControllerTests
    {
        private readonly ProductSearchController _controller;
        private readonly IProductSearchIndexService _productSearchIndexService;
        private readonly IProductSearchQueryService _productSearchQueryService;
        private CartModel _cartModel;

        public ProductSearchControllerTests()
        {
            _productSearchIndexService = A.Fake<IProductSearchIndexService>();
            _cartModel = new CartModel();
            _productSearchQueryService = A.Fake<IProductSearchQueryService>();
            _controller = new ProductSearchController(_productSearchIndexService, _cartModel, _productSearchQueryService)
            {
                RequestMock = A.Fake<HttpRequestBase>()
            };
        }

        private ProductSearch GetProductSearch()
        {
            return new ProductSearch { Layout = new Layout() };
        }

        [Fact]
        public void ProductSearchController_Show_ReturnsAViewResult()
        {
            ProductSearch productSearch = GetProductSearch();

            ViewResult show = _controller.Show(productSearch, new ProductSearchQuery());

            show.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductSearchController_Show_ReturnsProductSearch()
        {
            ProductSearch productSearch = GetProductSearch();

            ViewResult show = _controller.Show(productSearch, new ProductSearchQuery());

            show.Model.Should().BeOfType<ProductSearch>();
        }
    }
}