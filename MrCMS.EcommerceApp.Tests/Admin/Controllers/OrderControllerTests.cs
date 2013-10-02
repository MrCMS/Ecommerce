using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderControllerTests
    {
        private readonly IOrderService _orderService;
        private readonly OrderController _orderController;
        private readonly IOptionService _optionService;
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly IOrderSearchService _orderSearchService;
        private readonly IOrderShippingService _orderShippingService;
        private readonly EcommerceSettings _ecommerceSettings;

        public OrderControllerTests()
        {
            _orderService = A.Fake<IOrderService>();
            _optionService = A.Fake<IOptionService>();
            _shippingMethodManager = A.Fake<IShippingMethodManager>();
            _orderSearchService = A.Fake<IOrderSearchService>();
            _orderShippingService = A.Fake<IOrderShippingService>();
            _ecommerceSettings = new EcommerceSettings();
            _orderController = new OrderController(_orderService,
                _shippingMethodManager, _orderSearchService, _orderShippingService, _optionService, _ecommerceSettings);
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