using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderControllerTests
    {
        private readonly IOrderService _orderService;
        private readonly OrderController _orderController;

        public OrderControllerTests()
        {
            _orderService = A.Fake<IOrderService>();
            _orderController = new OrderController(_orderService);
        }

        [Fact]
        public void OrderController_Index_ReturnsAViewResult()
        {
            var result = _orderController.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrderController_Index_ShouldCallOrderServiceGetAllPagedWithPassedArgument()
        {
            var result = _orderController.Index(123);

            A.CallTo(() => _orderService.GetPaged(123, 10)).MustHaveHappened();
        }

        [Fact]
        public void OrderController_Index_ShouldReturnResultOfOrderServiceCallAsModel()
        {
            var pagedList = A.Fake<IPagedList<Order>>();
            A.CallTo(() => _orderService.GetPaged(123, 10)).Returns(pagedList);
            var result = _orderController.Index(123);

            result.Model.Should().Be(pagedList);
        }

        [Fact]
        public void OrderController_Edit_ReturnsAViewResult()
        {
            var order = new Order();

            var result = _orderController.Edit(order);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrderController_Edit_ReturnsThePassedObjectAsTheModel()
        {
            var order = new Order();

            var result = _orderController.Edit(order);

            result.As<ViewResult>().Model.Should().Be(order);
        }
    }
}