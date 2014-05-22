using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using Xunit;
using FluentAssertions;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class CartManagerTests : InMemoryDatabaseTest
    {
        private readonly CartModel _cartModel;
        private readonly ProductVariant _productVariant = new ProductVariant();
        private readonly CartManager _cartManager;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IEnumerable<ICartSessionKeyList> _cartSessionKeyLists;
        private readonly IGetUserGuid _getUserGuid;
        private readonly ICartBuilder _cartBuilder;

        public CartManagerTests()
        {
            _cartModel = new CartModel();
            Session.Transact(session => session.SaveOrUpdate(_productVariant));
            _cartSessionManager = A.Fake<ICartSessionManager>();
            _cartSessionKeyLists = A.Fake<IEnumerable<ICartSessionKeyList>>();
            _getUserGuid = A.Fake<IGetUserGuid>();
            _cartBuilder = A.Fake<ICartBuilder>();
            A.CallTo(() => _cartBuilder.BuildCart()).Returns(_cartModel);
            _cartManager = new CartManager(_cartBuilder, Session, _cartSessionManager, _cartSessionKeyLists, _getUserGuid);
        }
        [Fact]
        public void CartManager_AddToCart_AddsAnItemToTheCart()
        {
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };
            _cartManager.AddToCart(addToCartModel);

            _cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartManager_AddToCart_ShouldIncreaseAmountIfItAlreadyExists()
        {
            _cartModel.Items.Add(new CartItem { Item = _productVariant, Quantity = 1 });
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };

            _cartManager.AddToCart(addToCartModel);

            _cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartManager_AddToCart_ShouldPersistToDb()
        {
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };
            _cartManager.AddToCart(addToCartModel);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        [Fact]
        public void CartManager_AddToCart_WithExistingItemShouldOnlyHave1DbRecord()
        {
            _cartModel.Items.Add(new CartItem { Item = _productVariant, Quantity = 1 });
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };

            _cartManager.AddToCart(addToCartModel);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        [Fact]
        public void CartManager_Delete_ShouldRemoveCartItemFromModel()
        {
            var cartItem = new CartItem { Item = _productVariant, Quantity = 1 };
            _cartModel.Items.Add(cartItem);

            _cartManager.Delete(cartItem);

            _cartModel.Items.Should().HaveCount(0);
        }

        [Fact]
        public void CartManager_Delete_ShouldRemoveCartItemFromDb()
        {
            var cartItem = new CartItem { Item = _productVariant, Quantity = 1 };
            Session.Transact(session => session.Save(cartItem));
            _cartModel.Items.Add(cartItem);

            _cartManager.Delete(cartItem);

            Session.QueryOver<CartItem>().RowCount().Should().Be(0);
        }

        [Fact]
        public void CartManager_UpdateQuantity_ShouldUpdateQuantityValueOfItem()
        {
            var cartItem = new CartItem { Item = _productVariant, Quantity = 1 };
            Session.Transact(session => session.Save(cartItem));
            _cartModel.Items.Add(cartItem);

            _cartManager.UpdateQuantity(cartItem, 2);

            cartItem.Quantity.Should().Be(2);
        }

        [Fact]
        public void CartManager_EmptyBasket_RemovesItemsFromModel()
        {
            var cartItem = new CartItem { Item = _productVariant, Quantity = 1 };
            _cartModel.Items.Add(cartItem);

            _cartManager.EmptyBasket();

            _cartModel.Items.Should().BeEmpty();
        }

        [Fact]
        public void CartManager_EmptyBasket_RemovesItemsFromDB()
        {
            var cartItem = new CartItem { Item = _productVariant, Quantity = 1 };
            _cartModel.Items.Add(cartItem);

            _cartManager.EmptyBasket();

            Session.QueryOver<CartItem>().RowCount().Should().Be(0);
        }
    }
}