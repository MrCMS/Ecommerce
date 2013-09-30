using System;
using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using System.Linq;
using NHibernate;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductVariant : SiteEntity
    {
        public ProductVariant()
        {
            OptionValues = new List<ProductOptionValue>();
            PriceBreaks = new List<PriceBreak>();
        }
        [DisplayName("Price Pre Tax")]
        public virtual decimal PricePreTax
        {
            get
            {
                return TaxAwareProductPrice.GetPriceExcludingTax(BasePrice, TaxRate);
            }
        }

        public virtual decimal Weight { get; set; }
        [StringLength(400)]
        public virtual string Name { get; set; }
        public virtual string EditUrl { get { return Product.EditUrl; } }

        [Required]
        [DisplayName("Price")]
        public virtual decimal BasePrice { get; set; }

        [DisplayName("Previous Price")]
        public virtual decimal? PreviousPrice { get; set; }

        public virtual decimal? PreviousPriceIncludingTax
        {
            get { return TaxAwareProductPrice.GetPriceIncludingTax(PreviousPrice, TaxRate); }
        }

        public virtual decimal? PreviousPriceExcludingTax
        {
            get { return TaxAwareProductPrice.GetPriceExcludingTax(PreviousPrice, TaxRate); }
        }

        public virtual decimal ReducedBy
        {
            get
            {
                return PreviousPrice != null
                           ? PreviousPrice.Value > PricePreTax
                                 ? PreviousPrice.Value - PricePreTax
                                 : 0
                           : 0;
            }
        }

        public virtual decimal ReducedByPercentage
        {
            get
            {
                return PreviousPrice != null && PreviousPrice != 0
                           ? ReducedBy / PreviousPrice.Value
                           : 0;
            }
        }

        public virtual decimal Price
        {
            get { return TaxAwareProductPrice.GetPriceIncludingTax(BasePrice, TaxRate); }
        }

        private PriceBreak GetPriceBreak(int quantity)
        {
            return PriceBreaks != null
                       ? PriceBreaks.OrderBy(x => x.Price).FirstOrDefault(x => x.Quantity <= quantity)
                       : null;
        }

        public virtual decimal GetPrice(int quantity)
        {
            var priceBreak = GetPriceBreak(quantity);
            return priceBreak != null
                       ? priceBreak.PriceIncludingTax*quantity
                       : Price*quantity;
        }

        public virtual decimal GetUnitPrice(int quantity)
        {
            var priceBreak = GetPriceBreak(quantity);
            return priceBreak != null
                       ? priceBreak.PriceIncludingTax
                       : Price;
        }

        public virtual decimal GetUnitPricePreTax(int quantity)
        {
            var priceBreak = GetPriceBreak(quantity);
            return priceBreak != null
                       ? priceBreak.PriceExcludingTax
                       : PricePreTax;
        }

        public virtual decimal GetSaving(int quantity)
        {
            return PreviousPriceIncludingTax.GetValueOrDefault() != 0
                       ? ((PreviousPriceIncludingTax * quantity) - GetPrice(quantity)).Value
                       : (Price * quantity) - GetPrice(quantity);
        }

        public virtual decimal GetTax(int quantity)
        {
            return Math.Round(GetPrice(quantity) - GetPricePreTax(quantity), 2, MidpointRounding.AwayFromZero);
        }

        public virtual decimal GetPricePreTax(int quantity)
        {
            return Math.Round(GetPrice(quantity) / ((TaxRatePercentage + 100) / 100), 2, MidpointRounding.AwayFromZero);
        }


        [Required]
        [Remote("IsUniqueSKU", "ProductVariant", AdditionalFields = "Id")]
        public virtual string SKU { get; set; }

        public virtual decimal Tax { get { return Price - PricePreTax; } }

        public virtual bool CanBuy(int quantity)
        {
            return quantity > 0 && (!StockRemaining.HasValue || StockRemaining >= quantity);
        }

        public virtual ProductAvailability Availability
        {
            get
            {
                if (AvailableOn.HasValue && AvailableOn <= DateTime.UtcNow)
                    return ProductAvailability.Available;
                return ProductAvailability.PreOrder;
            }
        }

        [DisplayName("Available On")]
        public virtual DateTime? AvailableOn { get; set; }

        public virtual bool InStock
        {
            get { return !StockRemaining.HasValue || StockRemaining > 0; }
        }
        [DisplayName("Stock Remaining")]
        public virtual int? StockRemaining { get; set; }

        public virtual Product Product { get; set; }

        public virtual IList<ProductOptionValue> OptionValues { get; set; }
        public virtual IEnumerable<ProductOptionValue> AttributeValuesOrdered { get { return OptionValues.OrderBy(value => value.DisplayOrder); } }
        public virtual IList<PriceBreak> PriceBreaks { get; set; }

        [StringLength(200)]
        public virtual string Barcode { get; set; }
        [DisplayName("Tracking Policy")]
        public virtual TrackingPolicy TrackingPolicy { get; set; }

        [DisplayName("Tax Rate")]
        public virtual TaxRate TaxRate { get; set; }
        
        
        [DisplayName("Manufacturer Part Number")]
        [StringLength(250)]
        public virtual string ManufacturerPartNumber { get; set; }

        public virtual decimal TaxRatePercentage
        {
            get
            {
                return MrCMSApplication.Get<TaxSettings>().TaxesEnabled
                           ? TaxRate == null
                                 ? 0
                                 : TaxRate.Percentage
                           : 0;
            }
        }

        public virtual string DisplayName
        {
            get { return !string.IsNullOrWhiteSpace(Name) ? Name : Product.Name; }
        }
        public virtual string SelectOptionName
        {
            get
            {
                var title = string.Empty;
                if (!string.IsNullOrWhiteSpace(Name)) title = Name + " - ";

                if (OptionValues.Any())
                {
                    title += string.Join(", ", AttributeValuesOrdered.Select(value => value.Value));
                }

                title += " - " + Price.ToCurrencyFormat();

                return title;
            }
        }

        public virtual bool ShowPreviousPrice
        {
            get { return PreviousPrice.HasValue & PreviousPrice > Price; }
        }

        public virtual GoogleBaseProduct GoogleBaseProduct { get; set; }
    }
}