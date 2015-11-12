using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class CartItemBuilder
    {
        private bool? _canBuy;
        private decimal _discountAmount;
        private decimal _discountPercentage;
        private decimal? _pricePreTax;
        private int _quantity = 1;
        private decimal? _tax;
        private ProductVariant _variant = new ProductVariant();

        public CartItemData Build()
        {
            var testableCartItem = new TestableCartItem(_canBuy, _pricePreTax, _tax)
            {
                Quantity = _quantity,
                Item = _variant
            };
            testableCartItem.SetDiscountAmount(_discountAmount);
            testableCartItem.SetDiscountPercentage(_discountPercentage);
            return testableCartItem;
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

        public CartItemBuilder WithDiscountAmount(decimal discountAmount)
        {
            _discountAmount = discountAmount;
            return this;
        }

        public CartItemBuilder WithDiscountPercentage(decimal discountPercentage)
        {
            _discountPercentage = discountPercentage;
            return this;
        }
    }

    [DoNotMap]
    public class TestableCartItem : CartItemData
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

        public override bool CanBuy
        {
            get { return _canBuy ?? base.CanBuy; }
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