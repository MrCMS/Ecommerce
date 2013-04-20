using FakeItEasy;
using MrCMS.Entities.People;
using MrCMS.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Xunit;
using System.Collections;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OrderServiceTests : InMemoryDatabaseTest
    {
        public OrderServiceTests()
        {
            var currentUser = new User();
            Session.Transact(session => session.Save(currentUser));
            CurrentRequestData.CurrentUser = currentUser;
            A.CallTo(() => CurrentRequestData.CurrentContext.Session).Returns(new FakeHttpSessionState());
        }

        [Fact]
        public void OrderService_PlaceOrder_SavesThePassedOrderToSession()
        {
            OrderService orderService = new OrderService(Session, GetGetCartImpl());

            orderService.PlaceOrder(new CartModel());

            Session.QueryOver<Order>().RowCount().Should().Be(1);
        }

        GetCartImpl GetGetCartImpl()
        {
            return new GetCartImpl(Session);
        }
    }
}
