using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Cart
{
    public class CartItem : SiteEntity
    {
        private decimal _discountAmount = decimal.Zero;
        private int _freeItems;
        public virtual ProductVariant Item { get; set; }
        public virtual Guid UserGuid { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string Data { get; set; }
        public virtual CanBuyStatus CanBuyStatus { get; set; }

        public virtual int PricedQuantity
        {
            get { return Quantity - _freeItems; }
        }

        public virtual decimal Price
        {
            get { return UnitPrice * PricedQuantity; }
        }

        public virtual decimal PricePreDiscount
        {
            get { return UnitPricePreDiscount * Quantity; }
        }

        public virtual decimal Tax
        {
            get { return UnitTax * PricedQuantity; }
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
            get
            {
                var unitPrice = UnitPricePreDiscount - _discountAmount;
                // Ensure that it is at least free, not negative price
                return Math.Max(unitPrice, decimal.Zero);
            }
        }

        private decimal UnitPricePreDiscount
        {
            get { return Item.GetUnitPrice(Quantity); }
        }

        public virtual decimal UnitPricePreTax
        {
            get { return UnitPrice - UnitTax; }
        }

        public virtual decimal UnitTax
        {
            get
            {
                return Math.Round(UnitPrice * (TaxRatePercentage / (TaxRatePercentage + 100)), 2,
                    MidpointRounding.AwayFromZero);
            }
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
            get { return PricePreDiscount - Price; }
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
            get { return _discountAmount > 0 || _freeItems > 0; }
        }

        public virtual bool CanBuy
        {
            get { return CanBuyStatus.OK; }
        }

        public virtual string Error
        {
            get { return CanBuyStatus.Message; }
        }

        public virtual void SetDiscountAmount(decimal discountAmount)
        {
            _discountAmount += discountAmount;
        }

        public virtual void SetFreeItems(int freeItems)
        {
            _freeItems = freeItems;
        }
    }
}