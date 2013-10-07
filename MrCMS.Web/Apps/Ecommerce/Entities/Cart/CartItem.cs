using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Cart
{
    public class CartItem : SiteEntity
    {
        private Discount _discount;
        private string _discountCode;
        public virtual ProductVariant Item { get; set; }
        public virtual Guid UserGuid { get; set; }
        public virtual int Quantity { get; set; }

        public virtual decimal Price
        {
            get { return Item.GetPrice(Quantity) - DiscountAmount; }
        }
        public virtual decimal Saving
        {
            get { return Item.GetSaving(Quantity); }
        }
        public virtual decimal Tax
        {
            get { return Math.Round(Price*(TaxRatePercentage/(TaxRatePercentage + 100)), 2, MidpointRounding.AwayFromZero); }
        }
        public virtual bool CurrentlyAvailable
        {
            get { return Item.CanBuy(Quantity); }
        }
        public virtual decimal PricePreTax
        {
            get { return Price - Tax; }
        }
        public virtual decimal TaxRatePercentage
        {
            get { return Item.TaxRatePercentage; }
        }
        public virtual decimal Weight
        {
            get { return Item.Weight * Quantity; }
        }
        public virtual string Name
        {
            get { return Item.DisplayName; }
        }
        public virtual decimal UnitPrice
        {
            get { return Item.GetUnitPrice(Quantity); }
        }
        public virtual decimal UnitPricePreTax
        {
            get { return Item.GetUnitPricePreTax(Quantity); }
        }
        public virtual decimal DiscountAmount
        {
            get
            {
                return _discount != null
                           ? _discount.GetDiscount(this, _discountCode)
                           : decimal.Zero;
            }
        }
        public virtual void SetDiscountInfo(Discount discount, string discountCode)
        {
            _discount = discount;
            _discountCode = discountCode;
        }
    }
}