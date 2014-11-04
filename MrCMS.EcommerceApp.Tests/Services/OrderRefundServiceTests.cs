using FakeItEasy;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OrderRefundServiceTests : InMemoryDatabaseTest
    {
        private readonly OrderRefundService _orderRefundService;

        public OrderRefundServiceTests()
        {
            _orderRefundService = new OrderRefundService(Session);
        }

        [Fact]
        public void OrderRefundService_Add_ShouldCallOrderPartiallyRefunded()
        {
            var order = new Order {Total = 100};
            var orderRefund = new OrderRefund {Amount = 10, Order = order};

            _orderRefundService.Add(orderRefund);

            A.CallTo(
                () => 
                    EventContext.FakeEventContext.Publish<IOnOrderPartiallyRefunded, OrderPartiallyRefundedArgs>(
                        A<OrderPartiallyRefundedArgs>.That.Matches(args => args.Order == order))).MustHaveHappened();
        }

        [Fact]
        public void OrderRefundService_Add_ShouldCallOrderFullyRefunded()
        {
            var order = new Order {Total = 100};
            var orderRefund = new OrderRefund {Amount = 100, Order = order};

            _orderRefundService.Add(orderRefund);

            A.CallTo(
                () =>
                   EventContext.FakeEventContext.Publish<IOnOrderFullyRefunded, OrderFullyRefundedArgs>(
                        A<OrderFullyRefundedArgs>.That.Matches(args => args.Order == order))).MustHaveHappened();
        }
    }
}