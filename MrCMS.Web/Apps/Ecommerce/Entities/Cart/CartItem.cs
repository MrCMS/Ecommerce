using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Cart
{
    public class CartItem : SiteEntity
    {
        private decimal _discountAmount = decimal.Zero;
        public virtual ProductVariant Item { get; set; }
        public virtual Guid UserGuid { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string Data { get; set; }
        public virtual CanBuyStatus CanBuyStatus { get; set; }

        public virtual decimal Price
        {
            get { return PricePreDiscount - DiscountAmount; }
        }

        public virtual decimal PricePreDiscount
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
            get { return Item.Weight*Quantity; }
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

        public virtual bool RequiresShipping
        {
            get { return Item.RequiresShipping; }
        }

        public virtual bool IsDownloadable
        {
            get { return Item.IsDownloadable; }
        }

        public virtual decimal DiscountAmount
        {
            get { return _discountAmount >= PricePreDiscount ? PricePreDiscount : _discountAmount; }
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

        public virtual bool HasDiscount
        {
            get { return DiscountAmount > 0; }
        }

        public virtual bool CanBuy
        {
            get { return CanBuyStatus.OK; }
        }

        public virtual string Error
        {
            get { return CanBuyStatus.Message; }
        }

        public virtual void SetDiscountInfo(decimal discountAmount)
        {
            _discountAmount += discountAmount;
        }
    }
}