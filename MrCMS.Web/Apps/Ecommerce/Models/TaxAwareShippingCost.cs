using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class TaxAwareShippingRate
    {
        public static TaxAwareShippingRate Create(decimal? value, TaxRate rate, bool? shippingRateIncludesTax = null)
        {
            return PriceCreator(value, rate, shippingRateIncludesTax);
        }

        public static decimal GetPriceExcludingTax(decimal value, TaxRate rate, bool? shippingRateIncludesTax = null)
        {
            return PriceCreator(value, rate, shippingRateIncludesTax).PriceExcludingTax.GetValueOrDefault();
        }
        public static decimal GetPriceIncludingTax(decimal value, TaxRate rate, bool? shippingRateIncludesTax = null)
        {
            return PriceCreator(value, rate, shippingRateIncludesTax).PriceIncludingTax.GetValueOrDefault();
        }
        public static decimal? GetPriceExcludingTax(decimal? value, TaxRate rate, bool? shippingRateIncludesTax = null)
        {
            return PriceCreator(value, rate, shippingRateIncludesTax).PriceExcludingTax;
        }
        public static decimal? GetPriceIncludingTax(decimal? value, TaxRate rate, bool? shippingRateIncludesTax = null)
        {
            return PriceCreator(value, rate, shippingRateIncludesTax).PriceIncludingTax;
        }

        public static Func<decimal?, TaxRate, bool?, TaxAwareShippingRate> PriceCreator =
            (arg1, rate, arg3) => new TaxAwareShippingRate(arg1, rate, arg3);

        private readonly decimal? _value;
        private readonly TaxRate _taxRate;
        private readonly bool? _shippingRateIncludesTax;

        protected decimal TaxRatePercentage
        {
            get { return _taxRate != null ? _taxRate.Percentage : 0m; }
        }

        public TaxAwareShippingRate(decimal? value, TaxRate taxRate, bool? shippingRateIncludesTax = null)
        {
            _value = value;
            _taxRate = taxRate;
            _shippingRateIncludesTax = shippingRateIncludesTax;
        }

        public decimal? PriceExcludingTax
        {
            get
            {
                return !_value.HasValue ? (decimal?)null :
                Math.Round(ShippingRateIncludesTax
                                          ? _value.Value / ((TaxRatePercentage + 100) / 100)
                                          : _value.Value, 2, MidpointRounding.AwayFromZero);
            }
        }

        private bool ShippingRateIncludesTax
        {
            get
            {
                return _shippingRateIncludesTax ?? (!MrCMSApplication.Get<TaxSettings>().TaxesEnabled ||
                       MrCMSApplication.Get<TaxSettings>().ShippingRateIncludesTax);
            }
        }

        public decimal? PriceIncludingTax
        {
            get
            {
                return !_value.HasValue
                           ? (decimal?)null
                           : Math.Round(ShippingRateIncludesTax
                                            ? _value.Value
                                            : _taxRate != null
                                                  ? _value.Value * (_taxRate.Multiplier)
                                                  : _value.Value, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}