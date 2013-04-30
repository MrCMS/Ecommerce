using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Payments;
using MrCMS.EcommerceApp.Tests.Services;
using MrCMS.Entities.People;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderLineControllerTests : InMemoryDatabaseTest
    {
        private readonly IOrderLineService _orderLineService;
        private readonly OrderLineController _orderLineController;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderLineControllerTests()
        {
            A.CallTo(() => CurrentRequestData.CurrentContext.Session).Returns(new FakeHttpSessionState());
            _orderLineService = A.Fake<IOrderLineService>();
            _orderService = A.Fake<IOrderService>();
            _productService = A.Fake<IProductService>();
            _orderLineController = new OrderLineController(_orderLineService, _orderService,_productService);
        }

        [Fact]
        public void OrderLineController_Edit_ReturnsAViewResult()
        {
            var item = new OrderLine();
            item.ProductVariant = new Product() { Id = 1 };

            var result = _orderLineController.Edit(item);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrderLineController_Edit_ReturnsThePassedObjectAsTheModel()
        {
            var item = new OrderLine();
            item.ProductVariant = new Product() { Id = 1 };

            var result = _orderLineController.Edit(item);

            result.As<ViewResult>().Model.Should().Be(item);
        }
    }
}