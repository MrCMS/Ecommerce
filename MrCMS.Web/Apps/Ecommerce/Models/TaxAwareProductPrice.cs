using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class TaxAwareProductPrice
    {
        public static TaxAwareProductPrice Create(decimal? value, TaxRate rate, bool? loadedPricesIncludeTax = null)
        {
            return PriceCreator(value, rate, loadedPricesIncludeTax);
        }

        public static decimal GetPriceExcludingTax(decimal value, TaxRate rate, bool? loadedPricesIncludeTax = null)
        {
            return PriceCreator(value, rate, loadedPricesIncludeTax).PriceExcludingTax.GetValueOrDefault();
        }
        public static decimal GetPriceIncludingTax(decimal value, TaxRate rate, bool? loadedPricesIncludeTax = null)
        {
            return PriceCreator(value, rate, loadedPricesIncludeTax).PriceIncludingTax.GetValueOrDefault();
        }
        public static decimal? GetPriceExcludingTax(decimal? value, TaxRate rate, bool? loadedPricesIncludeTax = null)
        {
            return PriceCreator(value, rate, loadedPricesIncludeTax).PriceExcludingTax;
        }
        public static decimal? GetPriceIncludingTax(decimal? value, TaxRate rate, bool? loadedPricesIncludeTax = null)
        {
            return PriceCreator(value, rate, loadedPricesIncludeTax).PriceIncludingTax;
        }

        public static Func<decimal?, TaxRate, bool?, TaxAwareProductPrice> PriceCreator =
            (arg1, rate, arg3) => new TaxAwareProductPrice(arg1, rate, arg3);

        private readonly decimal? _value;
        private readonly TaxRate _taxRate;
        private readonly bool? _loadedPricesIncludeTax;

        protected decimal TaxRatePercentage
        {
            get { return _taxRate != null ? _taxRate.Percentage : 0m; }
        }

        public TaxAwareProductPrice(decimal? value, TaxRate taxRate, bool? loadedPricesIncludeTax = null)
        {
            _value = value;
            _taxRate = taxRate;
            _loadedPricesIncludeTax = loadedPricesIncludeTax;
        }

        public decimal? PriceExcludingTax
        {
            get
            {
                return !_value.HasValue ? (decimal?)null :
                Math.Round(LoadedPricesIncludeTax
                                          ? _value.Value / ((TaxRatePercentage + 100) / 100)
                                          : _value.Value, 2, MidpointRounding.AwayFromZero);
            }
        }

        private bool LoadedPricesIncludeTax
        {
            get
            {
                return _loadedPricesIncludeTax ?? (!MrCMSApplication.Get<TaxSettings>().TaxesEnabled ||
                        MrCMSApplication.Get<TaxSettings>().LoadedPricesIncludeTax);
            }
        }

        public decimal? PriceIncludingTax
        {
            get
            {
                return !_value.HasValue
                           ? (decimal?)null
                           : Math.Round(LoadedPricesIncludeTax
                                            ? _value.Value
                                            : _taxRate != null
                                                  ? _value.Value * (_taxRate.Multiplier)
                                                  : _value.Value, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}