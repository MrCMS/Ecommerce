using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class CartItemBuilder
    {
        private bool? _canBuy;
        private int _quantity = 1;
        private ProductVariant _variant = new ProductVariant();

        public CartItem Build()
        {
            return new TestableCartItem(_canBuy)
                       {
                           Quantity = _quantity,
                           Item = _variant
                       };
        }

        public CartItemBuilder WithItem(ProductVariant variant)
        {
            _variant = variant;
            return this;
        }

        public CartItemBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        public CartItemBuilder CanBuy()
        {
            _canBuy = true;
            return this;
        }

        public CartItemBuilder CannotBuy()
        {
            _canBuy = false;
            return this;
        }
    }

    [DoNotMap]
    public class TestableCartItem : CartItem
    {
        private readonly bool? _canBuy;

        public TestableCartItem(bool? canBuy = null)
        {
            _canBuy = canBuy;
        }

        public override bool CanBuy(CartModel cartModel)
        {
            return _canBuy ?? base.CanBuy(cartModel);
        }
    }
}