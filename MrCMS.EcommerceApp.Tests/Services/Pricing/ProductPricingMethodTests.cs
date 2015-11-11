using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.Pricing
{
    public class ProductPricingMethodTests
    {
        private const decimal TaxPercentage = 20;

        [Fact]
        public void IfTaxesAreDisabledTheBasePriceShouldBeThePrice()
        {
            var pricingMethod = new ProductPricingMethodBuilder().TaxesDisabled().Build();
            var productVariant = new ProductVariantBuilder().WithBasePrice(1.00m).Build();

            pricingMethod.GetUnitPrice(productVariant).Should().Be(1.00m);
        }

        [Fact]
        public void IfTaxesAreEnabledButPricesIncludeTaxTheBasePriceShouldBeThePrice()
        {
            var pricingMethod = new ProductPricingMethodBuilder().LoadedPricesIncludeTax().Build();
            var productVariant = new ProductVariantBuilder().WithBasePrice(1.00m).Build();

            pricingMethod.GetUnitPrice(productVariant).Should().Be(1.00m);
        }

        [Fact]
        public void IfPricesExcludeTaxTheyShouldBeAdded()
        {
            var pricingMethod = new ProductPricingMethodBuilder().LoadedPricesExcludeTax().Build();
            var productVariant = new ProductVariantBuilder().WithBasePrice(1.00m).WithTaxPercentage(20m).Build();

            pricingMethod.GetUnitPrice(productVariant).Should().Be(1.20m);
        }

        [Theory]
        [InlineData(false, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 0)]

        // exclusive of tax
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 0.2)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 5, 1)]
        // should round tax for 1.01 to 20p then * 5 = 1.00
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.01, 5, 1)]
        // should calculate tax for 5.05 to the penny - 1.01
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Row, 1.01, 5, 1.01)]

        // inclusive of tax
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 1, 0.17)]
        // should round tax on 1.00 to 17p then * 5 = 0.85
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 5, 0.85)]
        // should calculate tax on 5.00 as 0.83
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Row, 1, 5, 0.83)]
        public void TaxShouldBeCalculatedCorrectly(bool taxesEnabled, PriceLoadingMethod priceLoadingMethod,
            TaxCalculationMethod taxCalculationMethod,
            double basePrice, int quantity, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().TaxesEnabled(taxesEnabled)
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .WithTaxCalculationMethod(taxCalculationMethod).Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithQuantity(quantity).Build();

            pricingMethod.GetTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Theory]
        [InlineData(false, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 1)]

        // exclusive of tax
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 1)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 5, 5)]
        // should multiply up directly
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.01, 5, 5.05)]
        // should multiply up directly
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Row, 1.01, 5, 5.05)]

        // inclusive of tax
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 1, 0.83)]
        // should remove tax on 1.00 to 83p then * 5 = 4.15
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 5, 4.15)]
        // should remove tax on 5.00 removing 0.83 = 4.17
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Row, 1, 5, 4.17)]
        public void PricePreTaxShouldBeCalculatedCorrectly(bool taxesEnabled, PriceLoadingMethod priceLoadingMethod,
            TaxCalculationMethod taxCalculationMethod,
            double basePrice, int quantity, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().TaxesEnabled(taxesEnabled)
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .WithTaxCalculationMethod(taxCalculationMethod).Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithQuantity(quantity).Build();

            pricingMethod.GetPricePreTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Theory]
        [InlineData(false, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 1)]

        // exclusive of tax
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 1.20)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 5, 6)]
        // should multiply up directly - 1.21 * 5 = 6.05
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.01, 5, 6.05)]
        // should calculate tax on 5.05 to the penny - 1.01, so 6.06
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Row, 1.01, 5, 6.06)]

        // inclusive of tax
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 1, 1)]
        // should multiply up directly
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 5, 5)]
        // should multiply up directly
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Row, 1, 5, 5)]
        public void PriceShouldBeCalculatedCorrectly(bool taxesEnabled, PriceLoadingMethod priceLoadingMethod,
            TaxCalculationMethod taxCalculationMethod,
            double basePrice, int quantity, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().TaxesEnabled(taxesEnabled)
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .WithTaxCalculationMethod(taxCalculationMethod).Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(Convert.ToDecimal(TaxPercentage))
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithQuantity(quantity).Build();

            pricingMethod.GetPrice(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Theory]
        [InlineData(false, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 1, 1.23)]

        // exclusive of tax
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 1, 1.48)]
        // should be unaffected by quantity when whole numbers
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 5, 1.48)]
        // this should end up rounding to the same figure
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 5, 1.48)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Row, 1.23, 5, 1.48)]

        // inclusive of tax
        // should be the same regardless of quantity or tax calculation as it's the fixed round number
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1.23, 1, 1.23)]
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1.23, 5, 1.23)]
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Row, 1.23, 5, 1.23)]
        public void UnitPriceShouldBeCalculatedCorrectly(bool taxesEnabled, PriceLoadingMethod priceLoadingMethod,
            TaxCalculationMethod taxCalculationMethod,
            double basePrice, int quantity, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().TaxesEnabled(taxesEnabled)
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .WithTaxCalculationMethod(taxCalculationMethod).Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithQuantity(quantity).Build();

            pricingMethod.GetUnitPrice(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Theory]
        [InlineData(false, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 1, 1.23)]

        // exclusive of tax
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 1, 1.23)]
        // should be the same regardless of quantity or tax calculation as it's the fixed round number
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 5, 1.23)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1.23, 5, 1.23)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Row, 1.23, 5, 1.23)]

        // inclusive of tax
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1.23, 1, 1.02)]
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1.23, 5, 1.02)]
        // this should end up rounding to the same figure
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Row, 1.23, 5, 1.02)]
        public void UnitPricePreTaxShouldBeCalculatedCorrectly(bool taxesEnabled, PriceLoadingMethod priceLoadingMethod,
            TaxCalculationMethod taxCalculationMethod,
            double basePrice, int quantity, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().TaxesEnabled(taxesEnabled)
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .WithTaxCalculationMethod(taxCalculationMethod).Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithQuantity(quantity).Build();

            pricingMethod.GetUnitPricePreTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }


        [Theory]
        [InlineData(false, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 0)]

        // exclusive of tax
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 1, 0.2)]
        // should be the same regardless of quantity or tax calculation as it's the fixed round number
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 5, 0.2)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Individual, 1, 5, 0.2)]
        [InlineData(true, PriceLoadingMethod.ExcludingTax, TaxCalculationMethod.Row, 1, 5, 0.2)]

        // inclusive of tax
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 1, 0.17)]
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Individual, 1, 5, 0.17)]
        // this should end up rounding to the same figure
        [InlineData(true, PriceLoadingMethod.IncludingTax, TaxCalculationMethod.Row, 1, 5, 0.17)]
        public void UnitTaxShouldBeCalculatedCorrectly(bool taxesEnabled, PriceLoadingMethod priceLoadingMethod,
            TaxCalculationMethod taxCalculationMethod,
            double basePrice, int quantity, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().TaxesEnabled(taxesEnabled)
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .WithTaxCalculationMethod(taxCalculationMethod).Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithQuantity(quantity).Build();

            pricingMethod.GetUnitTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }
    }
}