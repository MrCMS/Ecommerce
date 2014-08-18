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
        private decimal? _pricePreTax = null;
        private decimal? _tax = null;

        public CartItem Build()
        {
            return new TestableCartItem(_canBuy,_pricePreTax,_tax)
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

        public CartItemBuilder WithPricePreTax(decimal pricePreTax)
        {
            _pricePreTax = pricePreTax;
            return this;
        }

        public CartItemBuilder WithTax(decimal tax)
        {
            _tax = tax;
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
        private readonly decimal? _pricePreTax;
        private readonly decimal? _tax;

        public TestableCartItem(bool? canBuy = null, decimal? pricePreTax = null, decimal? tax = null)
        {
            _canBuy = canBuy;
            _pricePreTax = pricePreTax;
            _tax = tax;
        }

        public override bool CanBuy(CartModel cartModel)
        {
            return _canBuy ?? base.CanBuy(cartModel);
        }

        public override decimal PricePreTax
        {
            get { return _pricePreTax ?? base.PricePreTax; }
        }

        public override decimal Tax
        {
            get { return _tax ?? base.Tax; }
        }

        public override decimal Price
        {
            get
            {
                if (_pricePreTax.HasValue || _tax.HasValue)
                    return _pricePreTax.GetValueOrDefault() + _tax.GetValueOrDefault();
                return base.Price;
            }
        }
    }
}