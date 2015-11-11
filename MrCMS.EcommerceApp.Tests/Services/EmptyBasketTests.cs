using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class EmptyBasketTests : InMemoryDatabaseTest
    {
        private readonly CartModel _cartModel;
        private readonly ICartBuilder _cartBuilder;
        private readonly ProductVariant _productVariant = new ProductVariant();
        private readonly IClearCartSessionKeys _clearCartSessionKeys;
        private readonly EmptyBasket _emptyBasket;

        public EmptyBasketTests()
        {
            _cartModel = new CartModel();
            Session.Transact(session => session.SaveOrUpdate(_productVariant));
            _cartBuilder = A.Fake<ICartBuilder>();
            A.CallTo(() => _cartBuilder.BuildCart()).Returns(_cartModel);
            _clearCartSessionKeys = A.Fake<IClearCartSessionKeys>();
            _emptyBasket = new EmptyBasket(_cartBuilder, Session, _clearCartSessionKeys);
        }

        [Fact]
        public void Empty_RemovesItemsFromModel()
        {
            var cartItem = new CartItemData { Item = _productVariant, Quantity = 1 };
            _cartModel.Items.Add(cartItem);

            _emptyBasket.Empty();

            _cartModel.Items.Should().BeEmpty();
        }

        [Fact]
        public void Empty_RemovesItemsFromDB()
        {
            var cartItem = new CartItemData { Item = _productVariant, Quantity = 1 };
            _cartModel.Items.Add(cartItem);

            _emptyBasket.Empty();

            Session.QueryOver<CartItem>().RowCount().Should().Be(0);
        }

        [Fact]
        public void Empty_CallsClearCartSessionKeys()
        {
            _emptyBasket.Empty();

            A.CallTo(() => _clearCartSessionKeys.Clear()).MustHaveHappened();
        }
    }
}