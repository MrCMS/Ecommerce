using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

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
            get { return Item.GetTax(Quantity); }
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
        public virtual decimal UnitTax
        {
            get { return Item.GetUnitTax(Quantity); }
        }

        public virtual bool IsDownloadable
        {
            get { return Item.IsDownloadable; }
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

        public virtual int? AllowedNumberOfDownloads
        {
            get { return Item.AllowedNumberOfDownloads; }
        }

        public virtual int? AllowedNumberOfDaysForDownload
        {
            get { return Item.AllowedNumberOfDaysForDownload; }
        }

        public virtual string DownloadFileUrl
        {
            get { return Item.DownloadFileUrl; }
        }

        public virtual IList<ShippingMethod> RestrictedShippingMethods
        {
            get { return Item.RestrictedShippingMethods; }
        }

        public virtual void SetDiscountInfo(Discount discount, string discountCode)
        {
            _discount = discount;
            _discountCode = discountCode;
        }
    }
}