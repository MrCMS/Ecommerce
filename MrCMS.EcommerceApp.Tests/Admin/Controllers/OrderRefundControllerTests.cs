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

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderRefundControllerTests : InMemoryDatabaseTest
    {
        private readonly IOrderRefundService _orderRefundService;
        private readonly OrderRefundController _orderRefundController;
        private readonly IOrderService _orderService;

        public OrderRefundControllerTests()
        {
            A.CallTo(() => CurrentRequestData.CurrentContext.Session).Returns(new FakeHttpSessionState());
            _orderRefundService = A.Fake<IOrderRefundService>();
            _orderService = A.Fake<IOrderService>();
            _orderRefundController = new OrderRefundController(_orderRefundService, _orderService);
        }

        [Fact]
        public void OrderRefundController_Add_ReturnsViewResult()
        {
            var add = _orderRefundController.Add(1);

            add.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrderRefundController_Add_ReturnsAOrderRefund()
        {
            var add = _orderRefundController.Add(1);

            add.Model.Should().BeOfType<OrderRefund>();
        }

        [Fact]
        public void OrderRefundController_AddPOST_CallsIOrderRefundServiceAddWithPassedOrderRefund()
        {
            var item = new OrderRefund();
            item.Order = new Order { Id = 1 };

            var add = _orderRefundController.Add_POST(item);

            A.CallTo(() => _orderRefundService.Save(item)).MustHaveHappened();
        }

        [Fact]
        public void OrderRefundController_Delete_ReturnsAPartialView()
        {
            var delete = _orderRefundController.Delete(new OrderRefund());

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void OrderRefundController_Delete_ShouldReturnPassedOrderRefundAsModel()
        {
            var item = new OrderRefund();

            var delete = _orderRefundController.Delete(item);

            delete.Model.Should().Be(item);
        }

        [Fact]
        public void OrderRefundController_DeletePOST_ShouldCallDeleteForThePassedOrderRefund()
        {
            var item = new OrderRefund();
            item.Order = new Order { Id = 1 };

            var delete = _orderRefundController.Delete_POST(item);

            A.CallTo(() => _orderRefundService.Delete(item)).MustHaveHappened();
        }

        [Fact]
        public void OrderRefundController_DeletePOST_RedirectsToEditOrderForTheIdOfOrder()
        {
            var orderRefund = new OrderRefund();
            orderRefund.Order = new Order { Id = 1 };

            var delete = _orderRefundController.Delete_POST(orderRefund);

            delete.RouteValues["action"].Should().Be("Edit");
            delete.RouteValues["controller"].Should().Be("Order");
            delete.RouteValues["id"].Should().Be(1);
        }
    }
}