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
        public void OrderService_PlaceOrder()
        {

        }

        GetCartImpl GetGetCartImpl()
        {
            return new GetCartImpl(Session);
        }
    }
}
