using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.EcommerceApp.Tests.Helpers;
using Xunit.Extensions;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingCalculation_TaxTests : ShippingCalculationTests
    {
        [Theory]
        [InlineData(false, false, 5, 20, 0)]
        [InlineData(false, true, 5, 20, 0)]
        [InlineData(true, false, 5, 20, 1)]
        [InlineData(true, true, 6, 20, 1)]
        public void ShouldBeCalculatedCorrectly(bool taxesEnabled, bool priceIncludesTax, double baseAmount, double taxRate, double expected)
        {
            TaxSettings.TaxesEnabled = taxesEnabled;
            TaxSettings.ShippingRateTaxesEnabled = taxesEnabled;
            TaxSettings.ShippingRateIncludesTax = priceIncludesTax;
            var shippingCalculation = new ShippingCalculationBuilder()
                                            .WithBaseAmount(baseAmount.ToDecimal())
                                            .WithTaxRate(taxRate.ToDecimal())
                                            .Build();

            var tax = shippingCalculation.Tax;

            tax.Should().Be(expected.ToDecimal());
        }
    }
}