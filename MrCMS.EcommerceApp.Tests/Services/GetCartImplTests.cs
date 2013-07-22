using System;
using System.Collections;
using System.Linq;
using System.Web;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class GetCartImplTests : InMemoryDatabaseTest
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly GetCartImpl _getCartImpl;

        public GetCartImplTests()
        {
            var currentUser = new User();
            Session.Transact(session => session.Save(currentUser));
            CurrentRequestData.CurrentUser = currentUser;
            A.CallTo(() => CurrentRequestData.CurrentContext.Session).Returns(new FakeHttpSessionState());
            _paymentMethodService = A.Fake<IPaymentMethodService>();
            _getCartImpl = new GetCartImpl(Session,_paymentMethodService);
        }
        [Fact]
        public void GetCartImpl_GetCart_ReturnsACartModel()
        {
            var cartModel = _getCartImpl.GetCart();

            cartModel.Should().BeOfType<CartModel>();
        }

        [Fact]
        public void GetCartImpl_GetCart_ShouldReturnUserIfCurrentUserIsSet()
        {
            var cartModel = _getCartImpl.GetCart();

            cartModel.User.Should().Be(CurrentRequestData.CurrentUser);
        }

        [Fact]
        public void GetCartImpl_GetCart_ShouldReturnCartItemsIfTheyMatchUserGuid()
        {
            var cartItems = Enumerable.Range(1, 10)
                                      .Select(i =>
                                              new CartItem
                                                  {
                                                      UserGuid = CurrentRequestData.UserGuid
                                                  }).ToList();
            Session.Transact(session => cartItems.ForEach(item => session.Save(item)));

            var cartModel = _getCartImpl.GetCart();

            cartModel.Items.ShouldBeEquivalentTo(cartItems);
        }

        [Fact]
        public void GetCartImpl_GetCart_IfCurrentUserIsNullModelUserShouldBeNull()
        {
            CurrentRequestData.CurrentUser = null;

            var cartModel = _getCartImpl.GetCart();

            cartModel.User.Should().BeNull();
        }

        [Fact]
        public void GetCartImpl_GetCart_IfCurrentUserIsNullUserGuidShouldStillNotBeEmpty()
        {
            CurrentRequestData.CurrentUser = null;
            var userGuid = Guid.NewGuid();
            CurrentRequestData.UserGuid = userGuid;

            var cartModel = _getCartImpl.GetCart();

            cartModel.UserGuid.Should().Be(userGuid);
        }
    }

    public class FakeHttpSessionState : HttpSessionStateBase
    {
        public FakeHttpSessionState()
        {
            _store = new SortedList();
        }

        public override object this[string name]
        {
            get
            {
                return _store[name];
            }
            set
            {
                _store[name] = value;
            }
        }
        private readonly SortedList _store;
    }
}