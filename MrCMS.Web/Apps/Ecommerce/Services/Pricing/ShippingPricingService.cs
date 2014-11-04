using System;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.Pricing
{
    public class ShippingPricingService : IShippingPricingService
    {
        private readonly TaxSettings _taxSettings;

        public ShippingPricingService(TaxSettings taxSettings)
        {
            _taxSettings = taxSettings;
        }

        public decimal GetPriceIncludingTax(decimal? amount, decimal taxRate)
        {
            if (!TaxesEnabled)
                return amount.GetValueOrDefault();
            return !amount.HasValue
                ? decimal.Zero
                : Math.Round(_taxSettings.ShippingRateIncludesTax
                    ? amount.Value
                    : amount.Value*(1 + (taxRate/100m)),
                        2, MidpointRounding.AwayFromZero);
        }

        public decimal GetPriceExcludingTax(decimal? amount, decimal taxRate)
        {
            return amount.HasValue
                ? GetPriceIncludingTax(amount, taxRate) - GetTax(amount, taxRate)
                : decimal.Zero;
        }

        public decimal GetTax(decimal? amount, decimal taxRate)
        {
            if (!TaxesEnabled)
                return decimal.Zero;
            return !amount.HasValue
                ? decimal.Zero
                : Math.Round(_taxSettings.ShippingRateIncludesTax
                    ? amount.Value * (taxRate / (taxRate + 100))
                    : amount.Value * (taxRate / 100), 2,
                    MidpointRounding.AwayFromZero);
        }

        private bool TaxesEnabled
        {
            get { return _taxSettings.TaxesEnabled && _taxSettings.ShippingRateTaxesEnabled; }
        }
    }
}