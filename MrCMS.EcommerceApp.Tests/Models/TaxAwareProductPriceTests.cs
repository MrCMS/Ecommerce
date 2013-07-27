using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models
{
    public class TaxAwareProductPriceTests
    {
        [Fact]
        public void TaxAwareProductPrice_PriceExcludingTax_IsNullWhenPassedValueIsNull()
        {
            var taxAwarePrice = BuildTaxAwareProductPrice(null, 0m);

            taxAwarePrice.PriceExcludingTax.Should().Be(null);
        }

        [Fact]
        public void TaxAwareProductPrice_PriceIncludingTax_IsNullWhenPassedValueIsNull()
        {
            var taxAwarePrice = BuildTaxAwareProductPrice(null, 0m);

            taxAwarePrice.PriceIncludingTax.Should().Be(null);
        }

        [Fact]
        public void TaxAwareProductPrice_PriceIncludingTax_TaxesDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareProductPrice = BuildTaxAwareProductPrice(10, 20);

            taxAwareProductPrice.PriceIncludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareProductPrice_PriceExcludingTax_TaxesDisabledShouldBeTheSameAsPassedPrice()
        {
            var taxAwareProductPrice = BuildTaxAwareProductPrice(10, 20);

            taxAwareProductPrice.PriceExcludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareProductPrice_PriceIncludingTax_GlobalTaxesEnabledAndNotLoadedPricesIncludeTaxShouldBeThePassedPricePlusTax()
        {
            var taxAwareProductPrice = BuildTaxAwareProductPrice(10, 20, true, false);

            taxAwareProductPrice.PriceIncludingTax.Should().Be(12);
        }

        [Fact]
        public void TaxAwareProductPrice_PriceExcludingTax_GlobalTaxesEnabledAndNotLoadedPricesIncludeTaxShouldBeTheSameAsPassedPrice()
        {
            var taxAwareProductPrice = BuildTaxAwareProductPrice(10, 20, true, false);

            taxAwareProductPrice.PriceExcludingTax.Should().Be(10);
        }

        [Fact]
        public void TaxAwareProductPrice_PriceIncludingTax_GlobalTaxesEnabledAndLoadedPricesIncludeTaxShouldBeThePassedPrice()
        {
            var taxAwareProductPrice = BuildTaxAwareProductPrice(12, 20, true, true);

            taxAwareProductPrice.PriceIncludingTax.Should().Be(12);
        }

        [Fact]
        public void TaxAwareProductPrice_PriceExcludingTax_GlobalTaxesEnabledAndLoadedPricesIncludeTaxShouldBeTheSameAsPassedPriceLessTax()
        {
            var taxAwareProductPrice = BuildTaxAwareProductPrice(12, 20, true, true);

            taxAwareProductPrice.PriceExcludingTax.Should().Be(10);
        }

        private static TaxAwareProductPrice BuildTaxAwareProductPrice(decimal? value, decimal taxRatePercentage, bool taxesEnabled = false, 
                                                                      bool loadedPricesIncludeTax = false)
        {
            return TaxAwareProductPrice.Create(value, new TaxRate { Percentage = taxRatePercentage },
                                               new TaxSettings
                                               {
                                                   TaxesEnabled = taxesEnabled,
                                                   LoadedPricesIncludeTax = loadedPricesIncludeTax
                                               });
        }
    }
}