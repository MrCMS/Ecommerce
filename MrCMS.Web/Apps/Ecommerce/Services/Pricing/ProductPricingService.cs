using System;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.Pricing
{
    public class ProductPricingService : IProductPricingService
    {
        private readonly TaxSettings _taxSettings;

        public ProductPricingService(TaxSettings taxSettings)
        {
            _taxSettings = taxSettings;
        }

        public decimal GetPriceIncludingTax(decimal? amount, decimal taxRate)
        {
            if (!_taxSettings.TaxesEnabled)
                return amount.GetValueOrDefault();
            return !amount.HasValue
                ? decimal.Zero
                : Math.Round(_taxSettings.LoadedPricesIncludeTax
                    ? amount.Value
                    : amount.Value * (1 + (taxRate / 100m))
                    , 2, MidpointRounding.AwayFromZero);
        }

        public decimal GetPriceExcludingTax(decimal? amount, decimal taxRate)
        {
            return amount.HasValue
                ? GetPriceIncludingTax(amount, taxRate) - GetTax(amount, taxRate)
                : decimal.Zero;
        }

        public decimal GetTax(decimal? amount, decimal taxRate)
        {
            if (!_taxSettings.TaxesEnabled)
                return decimal.Zero;
            return !amount.HasValue
                ? decimal.Zero
                : Math.Round(_taxSettings.LoadedPricesIncludeTax
                    ? amount.Value*(taxRate/(taxRate + 100))
                    : amount.Value*(taxRate/100),
                    2, MidpointRounding.AwayFromZero);
        }
    }
}