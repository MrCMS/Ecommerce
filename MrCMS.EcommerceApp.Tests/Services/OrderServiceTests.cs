using System;
using FakeItEasy;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OrderAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly IFileService _fileService;
        private readonly OrderAdminService _orderAdminService;
        private readonly IOrderNoteService _orderNoteService;

        public OrderAdminServiceTests()
        {
            _orderAdminService = new OrderAdminService(null, Session, null);
        }

        [Fact]
        public void OrderAdminService_Cancel_ShouldCallOrderCancelled()
        {
            var order = new Order {IsCancelled = true};
            Session.Transact(session => session.Save(order));

            _orderAdminService.Cancel(order);

            A.CallTo(
                () =>
                    EventContext.FakeEventContext.Publish<IOnOrderCancelled, OrderCancelledArgs>(
                        A<OrderCancelledArgs>.That.Matches(args => args.Order == order))).MustHaveHappened();
        }

        [Fact]
        public void OrderAdminService_MarkAsShipped_ShouldCallOrderShipped()
        {
            var order = new Order {ShippingStatus = ShippingStatus.Shipped, ShippingDate = DateTime.UtcNow};
            Session.Transact(session => session.Save(order));

            _orderAdminService.MarkAsShipped(order);

            A.CallTo(
                () =>
                    EventContext.FakeEventContext.Publish<IOnOrderShipped, OrderShippedArgs>(
                        A<OrderShippedArgs>.That.Matches(args => args.Order == order))).MustHaveHappened();
        }
    }
}