using System;
using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using System.Linq;
using NHibernate;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductVariant : SiteEntity
    {
        public ProductVariant()
        {
            AttributeValues = new List<ProductAttributeValue>();
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
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public virtual decimal BasePrice { get; set; }

        [DisplayName("Previous Price")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public virtual decimal? PreviousPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        public virtual decimal? PreviousPriceIncludingTax
        {
            get { return TaxAwareProductPrice.GetPriceIncludingTax(PreviousPrice, TaxRate); }
        }

        [DisplayFormat(DataFormatString = "{0:n0}")]
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

        public virtual decimal GetPrice(int quantity)
        {
            if (PriceBreaks.Any())
            {
                List<PriceBreak> priceBreaks = PriceBreaks.Where(x => quantity >= x.Quantity).OrderBy(x => x.Price).ToList();
                if (priceBreaks.Any())
                    return priceBreaks.First().PriceIncludingTax * quantity;
            }

            return Price * quantity;
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

        public virtual decimal GetUnitPrice()
        {
            return Price;
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

        public virtual IList<ProductAttributeValue> AttributeValues { get; set; }
        public virtual IList<PriceBreak> PriceBreaks
        {
            get
            {
                return MrCMSApplication.Get<ISession>()
                                       .QueryOver<PriceBreak>()
                                       .Where(@break => @break.Item == this)
                                       .Cacheable()
                                       .List();
            }
        }

        [StringLength(200)]
        public virtual string Barcode { get; set; }
        [DisplayName("Tracking Policy")]
        public virtual TrackingPolicy TrackingPolicy { get; set; }

        [DisplayName("Tax Rate")]
        public virtual TaxRate TaxRate { get; set; }

        public virtual decimal TaxRatePercentage
        {
            get
            {
                return TaxRate == null
                           ? 0
                           : TaxRate.Percentage;
            }
        }
    }
}