using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Cart
{
    public class CartItem : SiteEntity
    {
        public virtual ProductVariant Item { get; set; }
        public virtual Guid UserGuid { get; set; }
        public virtual int Quantity { get; set; }

        public virtual decimal Price
        {
            get { return Item.GetPrice(Quantity); }
        }
        public virtual decimal Saving
        {
            get { return Item.GetSaving(Quantity); }
        }
        public virtual decimal Tax
        {
            get { return Item.GetTax(Quantity); }
        }
        public virtual bool CurrentlyAvailable
        {
            get { return Item.CanBuy(Quantity); }
        }
        public virtual decimal PricePreTax
        {
            get { return Item.GetPricePreTax(Quantity); }
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
            get { return Item.PricePreTax; }
        }
        public virtual decimal GetDiscountAmount(Discount discount, string discountCode)
        {
            return discount != null
                       ? discount.GetDiscount(this, discountCode)
                       : decimal.Zero;
        }
    }
}