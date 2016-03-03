using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class CartItemManagerTests : InMemoryDatabaseTest
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly CartItemManager _cartItemManager;
        private readonly CartModel _cartModel;
        private readonly IGetUserGuid _getUserGuid;
        private readonly ProductVariant _productVariant = new ProductVariant();
        private IGetExistingCartItem _getExistingCartItem;

        public CartItemManagerTests()
        {
            _cartModel = new CartModel();
            Session.Transact(session => session.SaveOrUpdate(_productVariant));
            _getUserGuid = A.Fake<IGetUserGuid>();
            _cartBuilder = A.Fake<ICartBuilder>();
            _getExistingCartItem = new GetExistingCartItem(Session);//A.Fake<IGetExistingCartItem>();
            A.CallTo(() => _cartBuilder.BuildCart()).Returns(_cartModel);
            _cartItemManager = new CartItemManager(_cartBuilder, Session, _getUserGuid, _getExistingCartItem);
        }

        [Fact]
        public void CartItemManager_AddToCart_AddsAnItemToTheCart()
        {
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };
            _cartItemManager.AddToCart(addToCartModel);

            _cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartItemManager_AddToCart_ShouldIncreaseAmountIfItAlreadyExists()
        {
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };
            // add an item
            _cartItemManager.AddToCart(addToCartModel);

            // add the same item again
            _cartItemManager.AddToCart(addToCartModel);

            _cartModel.Items.Should().HaveCount(1);
        }

        [Fact]
        public void CartItemManager_AddToCart_ShouldPersistToDb()
        {
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };
            _cartItemManager.AddToCart(addToCartModel);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        [Fact]
        public void CartItemManager_AddToCart_WithExistingItemShouldOnlyHave1DbRecord()
        {
            _cartModel.Items.Add(new CartItemData { Item = _productVariant, Quantity = 1 });
            var addToCartModel = new AddToCartModel { ProductVariant = _productVariant, Quantity = 1 };

            _cartItemManager.AddToCart(addToCartModel);

            Session.QueryOver<CartItem>().RowCount().Should().Be(1);
        }

        //[Fact]
        //public void CartItemManager_Delete_ShouldRemoveCartItemFromModel()
        //{
        //    var cartItem = new CartItem { Item = _productVariant, Quantity = 1 };
        //    _cartModel.Items.Add(cartItem);

        //    _cartItemManager.Delete(cartItem);

        //    _cartModel.Items.Should().HaveCount(0);
        //}

        [Fact]
        public void CartItemManager_Delete_ShouldRemoveCartItemFromDb()
        {
            var cartItem = new CartItem { Item = _productVariant, Quantity = 1 };
            Session.Transact(session => session.Save(cartItem));
            _cartModel.Items.Add(cartItem.GetCartItemData());

            _cartItemManager.Delete(cartItem);

            Session.QueryOver<CartItem>().RowCount().Should().Be(0);
        }
    }
}