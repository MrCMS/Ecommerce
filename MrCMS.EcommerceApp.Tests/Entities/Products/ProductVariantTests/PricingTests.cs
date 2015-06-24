using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.EcommerceApp.Tests.Entities.Products.ProductVariantTests
{
    public class PricingTests : MrCMSTest
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate = A.Fake<IGetDefaultTaxRate>();

        public PricingTests()
        {
            base.Kernel.SetTaxSettings();
            Kernel.Bind<IGetDefaultTaxRate>().ToConstant(_getDefaultTaxRate);
        }

        public static IEnumerable<object[]> TaxRates
        {
            get
            {
                yield return new object[] {new TaxRate {Percentage = 10}, 10m};
                yield return new object[] {new TaxRate {Percentage = 20}, 20m};
            }
        }


        [Fact]
        public void ProductVariant_ReducedBy_IsZeroIfPreviousPriceIsNotSet()
        {
            var variant = new ProductVariant {BasePrice = 1, PreviousPrice = null};

            decimal reducedPrice = variant.ReducedBy;

            reducedPrice.Should().Be(0);
        }

        [Fact]
        public void ProductVariant_ReducedBy_ShouldBeTheDifferenceIfThePreviousPriceIsGreaterThanThePrice()
        {
            var variant = new ProductVariant {BasePrice = 1, PreviousPrice = 2};

            decimal reducedPrice = variant.ReducedBy;

            reducedPrice.Should().Be(1);
        }

        [Fact]
        public void ProductVariant_ReducedBy_ShouldBeZeroIfPriceIsGreaterThanPreviousPrice()
        {
            var variant = new ProductVariant {BasePrice = 2, PreviousPrice = 1};

            decimal reducedPrice = variant.ReducedBy;

            reducedPrice.Should().Be(0);
        }

        [Fact]
        public void ProductVariant_ReducedByPercentage_ShouldBeTheReducedByAsAPercentageOfThePreviousPrice()
        {
            var variant = new ProductVariant {BasePrice = 1, PreviousPrice = 2};

            decimal reducedByPercentage = variant.ReducedByPercentage;

            reducedByPercentage.Should().Be(0.5m);
        }

        [Fact]
        public void ProductVariant_ReducedByPercentage_ShouldReturnZeroIfPreviousPriceIsNull()
        {
            var variant = new ProductVariant {BasePrice = 1};

            decimal reducedByPercentage = variant.ReducedByPercentage;

            reducedByPercentage.Should().Be(0);
        }

        [Fact]
        public void ProductVariant_Price_WithNoTaxRateShouldBePricePreTax()
        {
            var variant = new ProductVariant {BasePrice = 1};

            decimal price = variant.Price;

            price.Should().Be(1);
        }

        [Fact]
        public void ProductVariant_Price_WithTaxesEnabledAndRateSetShouldBeBasePricePlusTax()
        {
            Kernel.SetTaxSettings(true);
            var variant = new ProductVariant {BasePrice = 1, TaxRate = new TaxRate {Percentage = 20}};

            decimal price = variant.Price;

            price.Should().Be(1.2m);
        }

        [Fact]
        public void ProductVariant_Price_WithTaxesDisabledAndRateSetShouldBeTheSameAsPricePreTax()
        {
            Kernel.SetTaxSettings(taxesEnabled: false);
            var variant = new ProductVariant {BasePrice = 1, TaxRate = new TaxRate {Percentage = 20}};

            decimal price = variant.Price;

            price.Should().Be(1);
        }

        [Fact]
        public void ProductVariant_Price_ShouldBeRoundedTo2DecimalPlaces()
        {
            Kernel.SetTaxSettings(true);
            var variant = new ProductVariant {BasePrice = 1, TaxRate = new TaxRate {Percentage = 17.5m}};

            decimal price = variant.Price;

            price.Should().Be(1.18m);
        }

        [Fact]
        public void ProductVariant_Tax_IfTaxesAreEnabledShouldBePriceMinusPricePreTax()
        {
            Kernel.SetTaxSettings(true);
            var variant = new ProductVariant {BasePrice = 1, TaxRate = new TaxRate {Percentage = 17.5m}};

            decimal tax = variant.Tax;

            tax.Should().Be(0.18m);
        }

        [Fact]
        public void ProductVariant_Tax_IfTaxesAreDisabledShouldBeZero()
        {
            Kernel.SetTaxSettings(false);
            var variant = new ProductVariant {BasePrice = 1, TaxRate = new TaxRate {Percentage = 17.5m}};

            decimal tax = variant.Tax;

            tax.Should().Be(0m);
        }

        [Theory]
        [PropertyData("TaxRates")]
        public void ProductVariant_TaxRatePercentage_TaxesEnabledIsTakenFromTheTaxRatesPercentage(TaxRate rate,
            decimal expected)
        {
            Kernel.SetTaxSettings(true);
            var productVariant = new ProductVariant {TaxRate = rate};

            productVariant.TaxRatePercentage.Should().Be(expected);
        }

        [Theory]
        [PropertyData("TaxRates")]
        public void ProductVariant_TaxRatePercentage_TaxesDisabledIsZero(TaxRate rate, decimal expected)
        {
            Kernel.SetTaxSettings(false);
            var productVariant = new ProductVariant {TaxRate = rate};

            productVariant.TaxRatePercentage.Should().Be(0);
        }

        [Fact]
        public void ProductVariant_TaxRatePercentage_IsZeroWhenTaxRateIsNull()
        {
            var productVariant = new ProductVariant {TaxRate = null};

            productVariant.TaxRatePercentage.Should().Be(0);
        }


        [Fact]
        public void ProductVariant_PricePreTax_IfStoreSettingsAreNotLoadedPricesIncludeTaxShouldBeSameAsBasePrice()
        {
            Kernel.SetTaxSettings(true, false);
            var product = new ProductVariant {BasePrice = 6, TaxRate = new TaxRate {Percentage = 20}};

            product.PricePreTax.Should().Be(6);
        }

        [Fact]
        public void ProductVariant_PricePreTax_IfStoreSettingsAreLoadedPricesIncludeTaxShouldBeBasePriceLessVat()
        {
            Kernel.SetTaxSettings(true, true);
            var product = new ProductVariant {BasePrice = 6, TaxRate = new TaxRate {Percentage = 20}};

            product.PricePreTax.Should().Be(5);
        }

        [Fact]
        public void ProductVariant_Price_IfStoreSettingsAreNotLoadedPricesIncludeTaxShouldBeBasePricePlusVAT()
        {
            Kernel.SetTaxSettings(true, false);
            var product = new ProductVariant {BasePrice = 6, TaxRate = new TaxRate {Percentage = 20}};

            product.Price.Should().Be(7.2m);
        }

        [Fact]
        public void ProductVariant_Price_IfStoreSettingsAreLoadedPricesIncludeTaxShouldBeSameAsBasePrice()
        {
            Kernel.SetTaxSettings(true, true);
            var product = new ProductVariant {BasePrice = 6, TaxRate = new TaxRate {Percentage = 20}};

            product.Price.Should().Be(6);
        }

        [Fact]
        public void ProductVariant_GetUnitPrice_ShouldGetHighestQuantityMatchingPriceBreak()
        {
            Kernel.SetTaxSettings(true, true);
            var variant = new ProductVariant
            {
                BasePrice = 10,
                PriceBreaks = new List<PriceBreak>
                {
                    new PriceBreak {Quantity = 2, Price = 9},
                    new PriceBreak {Quantity = 3, Price = 8},
                    new PriceBreak {Quantity = 4, Price = 7}
                }
            };

            variant.GetUnitPrice(3).Should().Be(8);
        }
    }
}