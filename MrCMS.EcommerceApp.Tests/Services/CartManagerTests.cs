using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using NHibernate;
using Xunit;
using FluentAssertions;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class CartManagerTests : InMemoryDatabaseTest
    {
        private static IGetCart getCart;
        private CartModel cartModel;
        private Product product;

        [Fact]
        public void CartManager_AddToCart_AddsAnItemToTheCart()
        {
            CartManager cartManager = GetCartManager();

            cartManager.AddToCart(product, 1);

            cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartManager_AddToCart_ShouldIncreaseAmountIfItAlreadyExists()
        {
            CartManager cartManager = GetCartManager();
            cartModel.Items.Add(new CartItem { Item = product, Quantity = 1 });

            cartManager.AddToCart(product, 1);

            cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartManager_AddToCart_ShouldPersistToDb()
        {
            CartManager cartManager = GetCartManager();

            cartManager.AddToCart(product, 1);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        [Fact]
        public void CartManager_AddToCart_WithExistingItemShouldOnlyHave1DbRecord()
        {
            CartManager cartManager = GetCartManager();
            cartModel.Items.Add(new CartItem { Item = product, Quantity = 1 });

            cartManager.AddToCart(product, 1);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        [Fact]
        public void CartManager_Delete_ShouldRemoveCartItemFromModel()
        {
            CartManager cartManager = GetCartManager();
            var cartItem = new CartItem {Item = product, Quantity = 1};
            cartModel.Items.Add(cartItem);

            cartManager.Delete(cartItem);

            cartModel.Items.Should().HaveCount(0);
        }

        [Fact]
        public void CartManager_Delete_ShouldRemoveCartItemFromDb()
        {
            CartManager cartManager = GetCartManager();
            var cartItem = new CartItem {Item = product, Quantity = 1};
            Session.Transact(session => session.Save(cartItem));
            cartModel.Items.Add(cartItem);

            cartManager.Delete(cartItem);

            Session.QueryOver<CartItem>().RowCount().Should().Be(0);
        }

        [Fact]
        public void CartManager_UpdateQuantity_ShouldUpdateQuantityValueOfItem()
        {
            CartManager cartManager = GetCartManager();
            var cartItem = new CartItem {Item = product, Quantity = 1};
            Session.Transact(session => session.Save(cartItem));
            cartModel.Items.Add(cartItem);

            cartManager.UpdateQuantity(cartItem, 2);

            cartItem.Quantity.Should().Be(2);
        }

        [Fact]
        public void CartManager_UpdateQuantity_ShouldCallSessionUpdateOnTheItem()
        {
            var session = A.Fake<ISession>();
            CartManager cartManager = GetCartManager(session);
            var cartItem = new CartItem {Item = product, Quantity = 1};
            cartModel.Items.Add(cartItem);

            cartManager.UpdateQuantity(cartItem, 2);

            A.CallTo(() => session.Update(cartItem)).MustHaveHappened();
        }

        [Fact]
        public void CartManager_EmptyBasket_RemovesItemsFromModel()
        {
            var session = A.Fake<ISession>();
            CartManager cartManager = GetCartManager(session);
            var cartItem = new CartItem { Item = product, Quantity = 1 };
            cartModel.Items.Add(cartItem);

            cartManager.EmptyBasket();

            cartModel.Items.Should().BeEmpty();
        }

        [Fact]
        public void CartManager_EmptyBasket_RemovesItemsFromDB()
        {
            var session = A.Fake<ISession>();
            CartManager cartManager = GetCartManager(session);
            var cartItem = new CartItem { Item = product, Quantity = 1 };
            cartModel.Items.Add(cartItem);

            cartManager.EmptyBasket();

            Session.QueryOver<CartItem>().RowCount().Should().Be(0);
        }

        private CartManager GetCartManager(ISession session = null)
        {
            cartModel = new CartModel();
            getCart = A.Fake<IGetCart>();
            A.CallTo(() => getCart.GetCart()).Returns(cartModel);
            var currentSession = session ?? Session;
            product = new Product();
            currentSession.Transact(session1 => session1.SaveOrUpdate(product));
            return new CartManager(getCart, currentSession);
        }
    }
}