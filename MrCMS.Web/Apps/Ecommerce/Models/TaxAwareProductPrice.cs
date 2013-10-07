using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class TaxAwareProductPrice
    {
        public static TaxAwareProductPrice Create(decimal? value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings);
        }

        public static decimal GetPriceExcludingTax(decimal value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings).PriceExcludingTax.GetValueOrDefault();
        }
        public static decimal GetPriceIncludingTax(decimal value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings).PriceIncludingTax.GetValueOrDefault();
        }
        public static decimal? GetPriceExcludingTax(decimal? value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings).PriceExcludingTax;
        }
        public static decimal? GetPriceIncludingTax(decimal? value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings).PriceIncludingTax;
        }

        public static decimal GetTax(decimal value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings).Tax.GetValueOrDefault();
        }
        public static decimal? GetTax(decimal? value, TaxRate rate, TaxSettings settings = null)
        {
            return PriceCreator(value, rate, settings).Tax;
        }

        public static Func<decimal?, TaxRate, TaxSettings, TaxAwareProductPrice> PriceCreator =
            (arg1, rate, arg3) => new TaxAwareProductPrice(arg1, rate, arg3);

        private readonly decimal? _value;
        private readonly TaxRate _taxRate;
        private readonly TaxSettings _taxSettings;

        protected decimal TaxRatePercentage
        {
            get { return _taxRate != null ? _taxRate.Percentage : 0m; }
        }

        public TaxAwareProductPrice(decimal? value, TaxRate taxRate, TaxSettings taxSettings = null)
        {
            _value = value;
            _taxRate = taxRate;
            _taxSettings = taxSettings ?? MrCMSApplication.Get<TaxSettings>();
        }

        public decimal? PriceExcludingTax
        {
            get { return !_value.HasValue ? null : PriceIncludingTax - Tax; }
        }


        public decimal? Tax
        {
            get
            {
                if (!_taxSettings.TaxesEnabled)
                    return 0m;
                return !_value.HasValue
                           ? (decimal?) null
                           : Math.Round(_taxSettings.LoadedPricesIncludeTax
                                            ? _value.Value*(TaxRatePercentage/(TaxRatePercentage + 100))
                                            : _value.Value*(TaxRatePercentage/100), 2,
                                        MidpointRounding.AwayFromZero);
            }
        }


        public decimal? PriceIncludingTax
        {
            get
            {
                if (!_taxSettings.TaxesEnabled)
                    return _value;
                return !_value.HasValue
                           ? (decimal?)null
                           : Math.Round(_taxSettings.LoadedPricesIncludeTax
                                            ? _value.Value
                                            : _taxRate != null
                                                  ? _value.Value * (_taxRate.Multiplier)
                                                  : _value.Value, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}