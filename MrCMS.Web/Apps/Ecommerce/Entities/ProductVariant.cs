using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class ProductVariant : SiteEntity, ICanAddToCart
    {
        public virtual decimal PricePreTax { get; set; }

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

        public virtual DateTime? AvailableOn { get; set; }

        public virtual bool InStock
        {
            get { return !StockRemaining.HasValue || StockRemaining > 0; }
        }
        public virtual int? StockRemaining { get; set; }

    }
}