using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models
{
    public class TaxAwareShippingRateTests
    {
        [Fact]
        public void TaxAwareShippingRate_PriceIncludingTax_TaxesDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20);

            taxAwareShippingRate.PriceIncludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceExcludingTax_TaxesDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20);

            taxAwareShippingRate.PriceExcludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceIncludingTax_GlobalTaxesEnabledShippingDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20, true);

            taxAwareShippingRate.PriceIncludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceExcludingTax_GlobalTaxesEnabledShippingDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20, true);

            taxAwareShippingRate.PriceExcludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceIncludingTax_OnlyShippingTaxesEnabledShippingDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20, shippingTaxesEnabled: true);

            taxAwareShippingRate.PriceIncludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceExcludingTax_OnlyShippingTaxesEnabledShippingDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20, shippingTaxesEnabled: true);

            taxAwareShippingRate.PriceExcludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceIncludingTax_GlobalAndShippingTaxesEnabledAndNotShippingRatesIncludeTaxShouldBeThePassedPricePlusTax()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20, true, true);

            taxAwareShippingRate.PriceIncludingTax.Should().Be(12);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceExcludingTax_GlobalAndShippingTaxesEnabledAndNotShippingRatesIncludeTaxShouldBeTheSameAsPassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(10, 20, true, true);

            taxAwareShippingRate.PriceExcludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceIncludingTax_GlobalAndShippingTaxesEnabledAndShippingRatesIncludeTaxShouldBeThePassedPrice()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(12, 20, true, true, true);

            taxAwareShippingRate.PriceIncludingTax.Should().Be(12);
        }

        [Fact]
        public void TaxAwareShippingRate_PriceExcludingTax_GlobalAndShippingTaxesEnabledAndShippingRatesIncludeTaxShouldBeTheSameAsPassedPriceLessTax()
        {
            var taxAwareShippingRate = BuildTaxAwareShippingRate(12, 20, true, true, true);

            taxAwareShippingRate.PriceExcludingTax.Should().Be(10);
        }



        private static TaxAwareShippingRate BuildTaxAwareShippingRate(decimal? value, decimal taxRatePercentage, bool taxesEnabled = false, bool shippingTaxesEnabled = false,
                                                                      bool shippingRatesIncludeTax = false)
        {
            return TaxAwareShippingRate.Create(value, new TaxRate { Percentage = taxRatePercentage },
                                               new TaxSettings
                                                   {
                                                       TaxesEnabled = taxesEnabled,
                                                       ShippingRateTaxesEnabled = shippingTaxesEnabled,
                                                       ShippingRateIncludesTax = shippingRatesIncludeTax
                                                   });
        }
    }
}