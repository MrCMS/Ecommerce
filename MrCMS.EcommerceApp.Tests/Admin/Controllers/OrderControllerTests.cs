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

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderControllerTests
    {
        private readonly IOrderService _orderService;
        private readonly OrderController _orderController;
        private readonly IShippingStatusService _shippingStatusService;
        private readonly IPaymentStatusService _paymentStatusService;
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly IOrderSearchService _orderSearchService;
        private readonly IOrderShippingService _orderShippingService;

        public OrderControllerTests()
        {
            _orderService = A.Fake<IOrderService>();
            _shippingStatusService = A.Fake<IShippingStatusService>();
            _paymentStatusService = A.Fake<IPaymentStatusService>();
            _shippingMethodManager = A.Fake<IShippingMethodManager>();
            _orderSearchService = A.Fake<IOrderSearchService>();
            _orderShippingService = A.Fake<IOrderShippingService>();
            _orderController = new OrderController(_orderService, _shippingStatusService, _paymentStatusService,
                _shippingMethodManager, _orderSearchService, _orderShippingService);
        }

        //[Fact]
        //public void OrderController_Index_ReturnsAViewResult()
        //{
        //    var result = _orderController.Index();

        //    result.Should().BeOfType<ViewResult>();
        //}

        //[Fact]
        //public void OrderController_Index_ShouldCallOrderServiceGetAllPagedWithPassedArgument()
        //{
        //    var result = _orderController.Index(123);

        //    A.CallTo(() => _orderService.GetPaged(123, 10)).MustHaveHappened();
        //}

        //[Fact]
        //public void OrderController_Index_ShouldReturnResultOfOrderServiceCallAsModel()
        //{
        //    var pagedList = A.Fake<IPagedList<Order>>();
        //    A.CallTo(() => _orderService.GetPaged(123, 10)).Returns(pagedList);
        //    var result = _orderController.Index(123);

        //    result.Model.Should().Be(pagedList);
        //}

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