using System;
using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductVariant : SiteEntity, IBuyableItem
    {
        public ProductVariant()
        {
            AttributeValues = new List<ProductAttributeValue>();
            PriceBreaks = new List<PriceBreak>();
        }
        [DisplayName("Price Pre Tax")]
        public virtual decimal PricePreTax
        {
            get
            {
                return Math.Round(MrCMSApplication.Get<TaxSettings>().LoadedPricesIncludeTax
                                      ? BasePrice / ((TaxRatePercentage + 100) / 100)
                                      : BasePrice, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual decimal Weight { get; set; }
        public virtual string Name { get { return Product.Name; } }
        public virtual string EditUrl { get { return Product.EditUrl; } }

        [DisplayName("Price")]
        public virtual decimal BasePrice { get; set; }

        [DisplayName("Previous Price")]
        public virtual decimal? PreviousPrice { get; set; }

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
                return PreviousPrice != null
                           ? ReducedBy / PreviousPrice.Value
                           : 0;
            }
        }

        public virtual decimal Price
        {
            get
            {
                return Math.Round(TaxRate == null ? PricePreTax : PricePreTax * TaxRate.Multiplier, 2,
                                  MidpointRounding.AwayFromZero);
            }
        }

        public virtual decimal GetPrice(int quantity)
        {
            if (PriceBreaks.Count > 0)
            {
                List<PriceBreak> priceBreaks = PriceBreaks.Where(x => quantity >= x.Quantity).OrderBy(x => x.Price).ToList();
                if (priceBreaks.Count > 0)
                    return priceBreaks.First().GetPrice();
            }

            return Price;
        }

        public virtual decimal GetSaving(int quantity)
        {
            if (PreviousPrice != 0)
                return ((PreviousPrice * quantity) - (GetPrice(quantity) * quantity)).Value;
            else
                return GetPrice(quantity);
        }

        public virtual decimal GetTax(int quantity)
        {
            return Math.Round(MrCMSApplication.Get<TaxSettings>().LoadedPricesIncludeTax
                                       ? GetPrice(quantity) - (GetPrice(quantity) / ((TaxRatePercentage + 100) / 100))
                                       : 0, 2, MidpointRounding.AwayFromZero);
        }

        public virtual decimal GetPricePreTax(int quantity)
        {
            return Math.Round(MrCMSApplication.Get<TaxSettings>().LoadedPricesIncludeTax
                                       ? GetPrice(quantity) / ((TaxRatePercentage + 100) / 100)
                                       : GetPrice(quantity), 2, MidpointRounding.AwayFromZero);
        }


        public virtual IList<PriceBreak> PriceBreaks { get; set; }

        public virtual decimal GetPriceIncludingPriceBreaks(int quantity)
        {
            if (PriceBreaks.Count > 0)
            {
                List<PriceBreak> priceBreaks = PriceBreaks.Where(x => quantity >= x.Quantity).OrderBy(x => x.Price).ToList();
                if (priceBreaks.Count > 0)
                    return Math.Round(MrCMSApplication.Get<TaxSettings>().LoadedPricesIncludeTax
                                      ? priceBreaks.First().Price
                                      : TaxRate != null
                                            ? priceBreaks.First().Price * (TaxRate.Multiplier)
                                            : priceBreaks.First().Price, 2, MidpointRounding.AwayFromZero);
                else
                    return Price;
            }
            else
                return Price;
        }


        [DisplayName("Tax Rate")]
        public virtual TaxRate TaxRate { get; set; }

        public virtual decimal Tax { get { return Price - PricePreTax; } }
        public virtual string SKU { get; set; }
        public virtual decimal TaxRatePercentage
        {
            get
            {
                return TaxRate == null
                           ? 0
                           : TaxRate.Percentage;
            }
        }

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

    }
}