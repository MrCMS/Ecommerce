using System;
using FakeItEasy;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using NHibernate;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OrderServiceTests : MrCMSTest
    {
        private readonly IFileService _fileService;
        private readonly IOrderNoteService _orderNoteService;
        private readonly OrderService _orderService;
        private readonly ISession _session;

        public OrderServiceTests()
        {
            _session = A.Fake<ISession>();
            _orderNoteService = A.Fake<IOrderNoteService>();
            _fileService = A.Fake<IFileService>();
            _orderService = new OrderService(_session, _orderNoteService);
        }

        //TODO PlaceOrder

        [Fact]
        public void OrderService_Cancel_ShouldCallOrderCancelled()
        {
            var order = new Order {IsCancelled = true};

            _orderService.Cancel(order);

            A.CallTo(
                () =>
                    EventContext.FakeEventContext.Publish<IOnOrderCancelled, OrderCancelledArgs>(
                        A<OrderCancelledArgs>.That.Matches(args => args.Order == order))).MustHaveHappened();
        }

        [Fact]
        public void OrderService_MarkAsShipped_ShouldCallOrderShipped()
        {
            var order = new Order {ShippingStatus = ShippingStatus.Shipped, ShippingDate = DateTime.UtcNow};

            _orderService.MarkAsShipped(order);

            A.CallTo(
                () =>
                    EventContext.FakeEventContext.Publish<IOnOrderShipped, OrderShippedArgs>(
                        A<OrderShippedArgs>.That.Matches(args => args.Order == order))).MustHaveHappened();
        }
    }
}