using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingCalculation_AmountTests : ShippingCalculationTests
    {
        [Theory]
        [InlineData(false, false, false, 12)]
        [InlineData(true, false, false, 12)]
        [InlineData(true, true, false, 14.4)]
        [InlineData(true, true, true, 12)]
        public void ShouldCalculateAmountCorrectlyBasedOnTaxSettings(bool taxesEnabled, bool shippingTaxesEnabled, bool ratesIncludeTax, double expected)
        {
            TaxSettings.TaxesEnabled = taxesEnabled;
            TaxSettings.ShippingRateTaxesEnabled = shippingTaxesEnabled;
            TaxSettings.ShippingRateIncludesTax = ratesIncludeTax;
            ShippingCalculation shippingCalculation =
                new ShippingCalculationBuilder().WithBaseAmount(12).WithTaxRate(20).Build();

            shippingCalculation.Amount.Should().Be(Convert.ToDecimal(expected));
        }
    }
}