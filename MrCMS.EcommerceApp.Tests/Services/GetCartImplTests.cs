using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Services;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class GetCartImplTests : InMemoryDatabaseTest
    {
        private readonly CartBuilder _cartBuilder;
        private readonly IGetUserGuid _getUserGuid;

        public GetCartImplTests()
        {
            var currentUser = new User();
            Session.Transact(session => session.Save(currentUser));
            CurrentRequestData.CurrentUser = currentUser;
            _getUserGuid = A.Fake<IGetUserGuid>();
            _cartBuilder = new CartBuilder(A.Fake<IAssignBasicCartInfo>(), A.Fake<IAssignCartDiscountInfo>(),
                A.Fake<IAssignShippingInfo>(), A.Fake<IAssignPaymentInfo>(), _getUserGuid);
        }

        [Fact(Skip = "Refactoring")]
        public void GetCartImpl_GetCart_ReturnsACartModel()
        {
            CartModel cartModel = _cartBuilder.BuildCart();

            cartModel.Should().BeOfType<CartModel>();
        }

        [Fact(Skip = "Refactoring")]
        public void GetCartImpl_GetCart_ShouldReturnUserIfCurrentUserIsSet()
        {
            CartModel cartModel = _cartBuilder.BuildCart();

            cartModel.User.Should().Be(CurrentRequestData.CurrentUser);
        }

        [Fact(Skip = "Refactoring")]
        public void GetCartImpl_GetCart_ShouldReturnCartItemsIfTheyMatchUserGuid()
        {
            Guid newGuid = Guid.NewGuid();
            A.CallTo(() => _getUserGuid.UserGuid).Returns(newGuid);
            List<CartItem> cartItems = Enumerable.Range(1, 10)
                .Select(i =>
                    new CartItem
                    {
                        UserGuid = newGuid,
                        Item = new TestableProductVariant(true)
                    }).ToList();
            Session.Transact(session => cartItems.ForEach(item => session.Save(item)));

            CartModel cartModel = _cartBuilder.BuildCart();

            cartModel.Items.ShouldBeEquivalentTo(cartItems);
        }

        [Fact(Skip = "Refactoring")]
        public void GetCartImpl_GetCart_IfCurrentUserIsNullModelUserShouldBeNull()
        {
            CurrentRequestData.CurrentUser = null;

            CartModel cartModel = _cartBuilder.BuildCart();

            cartModel.User.Should().BeNull();
        }

        [Fact(Skip = "Refactoring")]
        public void GetCartImpl_GetCart_CartModelUserGuidShouldBeGetUserGuid()
        {
            Guid newGuid = Guid.NewGuid();
            A.CallTo(() => _getUserGuid.UserGuid).Returns(newGuid);

            CartModel cartModel = _cartBuilder.BuildCart();

            cartModel.UserGuid.Should().Be(newGuid);
        }
    }

    public class FakeHttpSessionState : HttpSessionStateBase
    {
        private readonly SortedList _store;

        public FakeHttpSessionState()
        {
            _store = new SortedList();
        }

        public override object this[string name]
        {
            get { return _store[name]; }
            set { _store[name] = value; }
        }
    }
}