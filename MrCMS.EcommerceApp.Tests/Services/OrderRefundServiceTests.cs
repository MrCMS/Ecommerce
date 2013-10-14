using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OrderRefundServiceTests : InMemoryDatabaseTest
    {
        private OrderRefundService _orderRefundService;
        private IOrderEventService _orderEventService;

        public OrderRefundServiceTests()
        {
            _orderEventService = A.Fake<IOrderEventService>();
            _orderRefundService = new OrderRefundService(Session, _orderEventService);
        }

        [Fact]
        public void OrderRefundService_Add_ShouldCallOrderPartiallyRefunded()
        {
            var order = new Order(){Total = 100};
            var orderRefund = new OrderRefund(){Amount = 10, Order = order};

            _orderRefundService.Add(orderRefund);

            A.CallTo(() => _orderEventService.OrderPartiallyRefunded(order,orderRefund)).MustHaveHappened();
        }

        [Fact]
        public void OrderRefundService_Add_ShouldCallOrderFullyRefunded()
        {
            var order = new Order() { Total = 100 };
            var orderRefund = new OrderRefund() { Amount = 100, Order = order };

            _orderRefundService.Add(orderRefund);

            A.CallTo(() => _orderEventService.OrderFullyRefunded(order, orderRefund)).MustHaveHappened();
        }
    }
}