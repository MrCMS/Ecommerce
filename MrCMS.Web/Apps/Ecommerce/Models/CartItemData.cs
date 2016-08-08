using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CartItemData
    {
        private decimal _discountAmount = decimal.Zero;
        private int _freeItems;
        private decimal _discountPercentage;
        public ProductVariant Item { get; set; }
        public Guid UserGuid { get; set; }
        public int Quantity { get; set; }
        public string Data { get; set; }
        public CanBuyStatus CanBuyStatus { get; set; }
        public IProductPricingMethod Pricing { get; set; }

        public void SetDiscountAmount(decimal discountAmount)
        {
            _discountAmount += discountAmount;
        }

        public void SetDiscountPercentage(decimal discountPercentage)
        {
            _discountPercentage += discountPercentage;

            // ensure that the value is between 0 and 100, to prevent strangeness
            _discountPercentage = _discountPercentage.Clamp(0m, 100m);
        }

        public void SetFreeItems(int freeItems)
        {
            _freeItems = freeItems;
        }

        public void ResetDiscountInfo()
        {
            _discountAmount = 0m;
            _freeItems = 0;
        }

        public string Name => Item.DisplayName;

        public int PricedQuantity => Quantity - _freeItems;

        public virtual decimal TaxRatePercentage => Pricing.GetTaxRatePercentage(Item);



        public decimal UnitPricePreDiscount =>  Pricing.GetUnitPricePreDiscount(this);
        // Item.GetUnitPrice(Quantity)

        public decimal UnitTax => Pricing.GetUnitTax(this);
        // Math.Round(UnitPrice * (TaxRatePercentage / (TaxRatePercentage + 100)), 2, MidpointRounding.AwayFromZero)

        // Ensure that it is at least free, not negative price
        public decimal UnitPrice => Pricing.GetUnitPrice(this);
        // Math.Max(UnitPricePreDiscount - _discountAmount, decimal.Zero);

        public decimal UnitPricePreTax => Pricing.GetUnitPricePreTax(this);
        //UnitPrice - UnitTax;



        public decimal PricePreDiscount => Pricing.GetPricePreDiscount(this);
            //UnitPricePreDiscount * Quantity;

        public virtual decimal Tax => Pricing.GetTax(this);
            //UnitTax * PricedQuantity;

        public virtual decimal Price => Pricing.GetPrice(this);
            //UnitPrice * PricedQuantity;

        public virtual decimal PricePreTax => Pricing.GetPricePreTax(this);
            //Price - Tax;

        public decimal DiscountAmount => PricePreDiscount - Price;
            //PricePreDiscount - Price;


        public decimal Weight => Item.Weight * Quantity;

        public bool RequiresShipping => Item.RequiresShipping;

        public bool IsDownloadable => Item.IsDownloadable;

        public int? AllowedNumberOfDownloads => Item.AllowedNumberOfDownloads;

        public int? AllowedNumberOfDaysForDownload => Item.AllowedNumberOfDaysForDownload;

        public string DownloadFileUrl => Item.DownloadFileUrl;

        public bool HasDiscount => _discountAmount > 0 || _freeItems > 0 || _discountPercentage > 0;

        public virtual bool CanBuy => CanBuyStatus.OK;

        public string Error => CanBuyStatus.Message;
        public int Id { get; set; }

        public decimal UnitDiscountAmount => _discountAmount;
        public decimal DiscountPercentage => _discountPercentage;

        public int FreeItems => _freeItems;

    }
}