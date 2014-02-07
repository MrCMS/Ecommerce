using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using Xunit.Extensions;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingCalculation_AmountPreTaxTests : ShippingCalculationTests
    {
        [Theory]
        [InlineData(false, false, false, 12)]
        [InlineData(true, false, false, 12)]
        [InlineData(true, true, false, 12)]
        [InlineData(true, true, true, 10)]
        public void ShouldCalculateAmountPreTaxCorrectlyBasedOnTaxSettings(bool taxesEnabled, bool shippingTaxesEnabled, bool ratesIncludeTax, double expected)
        {
            TaxSettings.TaxesEnabled = taxesEnabled;
            TaxSettings.ShippingRateTaxesEnabled = shippingTaxesEnabled;
            TaxSettings.ShippingRateIncludesTax = ratesIncludeTax;
            ShippingCalculation shippingCalculation =
                new ShippingCalculationBuilder().WithBaseAmount(12).WithTaxRate(20).Build();

            shippingCalculation.AmountPreTax.Should().Be(Convert.ToDecimal(expected));
        }
    }
}