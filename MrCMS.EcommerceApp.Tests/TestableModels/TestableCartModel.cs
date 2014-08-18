using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.TestableModels
{
    public class TestableCartModel : CartModel
    {
        private readonly decimal? _orderTotalDiscount;
        private readonly decimal? _shippingTotal;
        private readonly decimal? _itemTax;
        private readonly decimal? _shippingTax;
        private readonly decimal? _totalPreDiscount;
        private readonly decimal? _itemDiscount;
        private readonly bool? _requiresShipping;
        private readonly decimal? _total;
        private readonly decimal? _giftCardAmount;
        private readonly decimal? _totalPreShipping;
        private readonly decimal? _weight;

        public TestableCartModel(decimal? weight = null, decimal? totalPreShipping = null,
            decimal? orderTotalDiscount = null, decimal? shippingTotal = null,
            decimal? itemTax = null, decimal? shippingTax = null, decimal? totalPreDiscount = null,
            decimal? itemDiscount = null, bool? requiresShipping = null, decimal? total = null,
            decimal? giftCardAmount = null)
        {
            _weight = weight;
            _totalPreShipping = totalPreShipping;
            _orderTotalDiscount = orderTotalDiscount;
            _shippingTotal = shippingTotal;
            _itemTax = itemTax;
            _shippingTax = shippingTax;
            _totalPreDiscount = totalPreDiscount;
            _itemDiscount = itemDiscount;
            _requiresShipping = requiresShipping;
            _total = total;
            _giftCardAmount = giftCardAmount;
        }

        public override decimal Weight
        {
            get { return _weight ?? base.Weight; }
        }

        public override decimal TotalPreShipping
        {
            get { return _totalPreShipping ?? base.TotalPreShipping; }
        }

        public override decimal ShippingTotal
        {
            get { return _shippingTotal ?? base.ShippingTotal; }
        }

        public override decimal OrderTotalDiscount
        {
            get { return _orderTotalDiscount ?? base.OrderTotalDiscount; }
        }

        public override decimal ItemTax
        {
            get { return _itemTax ?? base.ItemTax; }
        }

        public override decimal ShippingTax
        {
            get { return _shippingTax ?? base.ShippingTax; }
        }

        public override decimal TotalPreDiscount
        {
            get { return _totalPreDiscount ?? base.TotalPreDiscount; }
        }

        public override decimal ItemDiscount
        {
            get { return _itemDiscount ?? base.ItemDiscount; }
        }

        public override bool RequiresShipping
        {
            get { return _requiresShipping ?? base.RequiresShipping; }
        }

        public override decimal Total
        {
            get { return _total ?? base.Total; }
        }

        public override decimal GiftCardAmount
        {
            get { return _giftCardAmount ?? base.GiftCardAmount; }
        }
    }
}