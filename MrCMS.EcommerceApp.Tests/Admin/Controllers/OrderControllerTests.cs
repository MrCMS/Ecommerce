using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderControllerTests
    {
        private readonly OrderController _orderController;
        private readonly IOrderAdminService _orderAdminService;

        public OrderControllerTests()
        {
            _orderAdminService = A.Fake<IOrderAdminService>();
            _orderController = new OrderController(_orderAdminService, A.Fake<IUserService>());
        }

        [Fact]
        public void OrderController_Index_ReturnsAViewResult()
        {
            var result = _orderController.Index(new OrderSearchModel());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrderController_Index_ShouldCallOrderServiceGetAllPagedWithPassedArgument()
        {
            var orderSearchModel = new OrderSearchModel();
            var result = _orderController.Index(orderSearchModel);

            A.CallTo(() => _orderAdminService.Search(orderSearchModel)).MustHaveHappened();
        }

        [Fact]
        public void OrderController_Index_ShouldReturnTheSearchModelAsTheModel()
        {
            var orderSearchModel = new OrderSearchModel();
            var result = _orderController.Index(orderSearchModel);

            result.Model.Should().Be(orderSearchModel);
        }

        [Fact]
        public void OrderController_Index_ShouldReturnTheResultOfTheCallToTheServiceAsViewData()
        {
            var orderSearchModel = new OrderSearchModel();
            var pagedList = PagedList<Order>.Empty;
            A.CallTo(() => _orderAdminService.Search(orderSearchModel)).Returns(pagedList);
            var result = _orderController.Index(orderSearchModel);

            result.ViewData["results"].Should().Be(pagedList);
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