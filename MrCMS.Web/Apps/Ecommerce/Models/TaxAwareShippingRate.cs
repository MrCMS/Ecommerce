using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class TaxAwareShippingRate
    {
        public static TaxAwareShippingRate Create(decimal? value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings);
        }

        public static decimal GetPriceExcludingTax(decimal value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate,  settings).PriceExcludingTax.GetValueOrDefault();
        }
        public static decimal GetPriceIncludingTax(decimal value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate,  settings).PriceIncludingTax.GetValueOrDefault();
        }
        public static decimal? GetPriceExcludingTax(decimal? value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate,  settings).PriceExcludingTax;
        }
        public static decimal? GetPriceIncludingTax(decimal? value, TaxRate rate,  TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings).PriceIncludingTax;
        }

        private static Func<decimal?, TaxRate, TaxSettings, TaxAwareShippingRate> PriceCreator =
            (arg1, rate, arg4) => new TaxAwareShippingRate(arg1, rate, arg4);

        private readonly decimal? _value;
        private readonly TaxRate _taxRate;
        private readonly TaxSettings _settings;

        private decimal TaxRatePercentage
        {
            get { return _taxRate != null ? _taxRate.Percentage : 0m; }
        }

        private TaxAwareShippingRate(decimal? value, TaxRate taxRate, TaxSettings settings = null)
        {
            _value = value;
            _taxRate = taxRate;
            _settings = settings ?? MrCMSApplication.Get<TaxSettings>();
        }

        public decimal? PriceExcludingTax
        {
            get
            {
                if (!TaxesEnabled)
                    return _value;

                return !_value.HasValue
                ? (decimal?)null :
                Math.Round(_settings.ShippingRateIncludesTax
                                          ? _value.Value / ((TaxRatePercentage + 100) / 100)
                                          : _value.Value, 2, MidpointRounding.AwayFromZero);
            }
        }

        public bool TaxesEnabled
        {
            get { return _settings.TaxesEnabled && _settings.ShippingRateTaxesEnabled; }
        }

        public decimal? PriceIncludingTax
        {
            get
            {
                if (!TaxesEnabled)
                    return _value;
                return !_value.HasValue
                           ? (decimal?)null
                           : Math.Round(_settings.ShippingRateIncludesTax
                                            ? _value.Value
                                            : _taxRate != null
                                                  ? _value.Value * (_taxRate.Multiplier)
                                                  : _value.Value, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}