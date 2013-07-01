using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using Xunit;
using FluentAssertions;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class CartManagerTests : InMemoryDatabaseTest
    {
        private static IGetCart _getCart;
        private CartModel _cartModel;
        private ProductVariant _productVariant = new ProductVariant();
        private CartManager _cartManager;

        public CartManagerTests()
        {
            _cartModel = new CartModel();
            _getCart = A.Fake<IGetCart>();
            A.CallTo(() => _getCart.GetCart()).Returns(_cartModel);
            Session.Transact(session1 => session1.SaveOrUpdate(_productVariant));
            _cartManager = new CartManager(_getCart, Session);
        }
        [Fact]
        public void CartManager_AddToCart_AddsAnItemToTheCart()
        {
            _cartManager.AddToCart(_productVariant, 1);

            _cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartManager_AddToCart_ShouldIncreaseAmountIfItAlreadyExists()
        {
            _cartModel.Items.Add(new CartItem { Item = _productVariant, Quantity = 1 });

            _cartManager.AddToCart(_productVariant, 1);

            _cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartManager_AddToCart_ShouldPersistToDb()
        {
            _cartManager.AddToCart(_productVariant, 1);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        [Fact]
        public void CartManager_AddToCart_WithExistingItemShouldOnlyHave1DbRecord()
        {
            _cartModel.Items.Add(new CartItem { Item = _productVariant, Quantity = 1 });

            _cartManager.AddToCart(_productVariant, 1);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        [Fact]
        public void CartManager_Delete_ShouldRemoveCartItemFromModel()
        {
            var cartItem = new CartItem {Item = _productVariant, Quantity = 1};
            _cartModel.Items.Add(cartItem);

            _cartManager.Delete(cartItem);

            _cartModel.Items.Should().HaveCount(0);
        }

        [Fact]
        public void CartManager_Delete_ShouldRemoveCartItemFromDb()
        {
            var cartItem = new CartItem {Item = _productVariant, Quantity = 1};
            Session.Transact(session => session.Save(cartItem));
            _cartModel.Items.Add(cartItem);

            _cartManager.Delete(cartItem);

            Session.QueryOver<CartItem>().RowCount().Should().Be(0);
        }

        [Fact]
        public void CartManager_UpdateQuantity_ShouldUpdateQuantityValueOfItem()
        {
            var cartItem = new CartItem {Item = _productVariant, Quantity = 1};
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