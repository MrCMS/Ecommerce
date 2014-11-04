using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.EcommerceApp.Tests.Entities.Products.ProductVariantTests
{
    public class PricingTests
    {
        private MockingKernel _mockingKernel;

        public PricingTests()
        {
            _mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(_mockingKernel);
            _mockingKernel.SetTaxSettings();
        }



        [Fact]
        public void ProductVariant_ReducedBy_IsZeroIfPreviousPriceIsNotSet()
        {
            var variant = new ProductVariant { BasePrice = 1, PreviousPrice = null };

            var reducedPrice = variant.ReducedBy;

            reducedPrice.Should().Be(0);
        }
        [Fact]
        public void ProductVariant_ReducedBy_ShouldBeTheDifferenceIfThePreviousPriceIsGreaterThanThePrice()
        {
            var variant = new ProductVariant { BasePrice = 1, PreviousPrice = 2 };

            var reducedPrice = variant.ReducedBy;

            reducedPrice.Should().Be(1);
        }
        [Fact]
        public void ProductVariant_ReducedBy_ShouldBeZeroIfPriceIsGreaterThanPreviousPrice()
        {
            var variant = new ProductVariant { BasePrice = 2, PreviousPrice = 1 };

            var reducedPrice = variant.ReducedBy;

            reducedPrice.Should().Be(0);
        }

        [Fact]
        public void ProductVariant_ReducedByPercentage_ShouldBeTheReducedByAsAPercentageOfThePreviousPrice()
        {
            var variant = new ProductVariant { BasePrice = 1, PreviousPrice = 2 };

            var reducedByPercentage = variant.ReducedByPercentage;

            reducedByPercentage.Should().Be(0.5m);
        }

        [Fact]
        public void ProductVariant_ReducedByPercentage_ShouldReturnZeroIfPreviousPriceIsNull()
        {
            var variant = new ProductVariant { BasePrice = 1 };

            var reducedByPercentage = variant.ReducedByPercentage;

            reducedByPercentage.Should().Be(0);
        }

        [Fact]
        public void ProductVariant_Price_WithNoTaxRateShouldBePricePreTax()
        {
            var variant = new ProductVariant { BasePrice = 1 };

            var price = variant.Price;

            price.Should().Be(1);
        }

        [Fact]
        public void ProductVariant_Price_WithTaxesEnabledAndRateSetShouldBeBasePricePlusTax()
        {
            _mockingKernel.SetTaxSettings(true);
            var variant = new ProductVariant { BasePrice = 1, TaxRate = new TaxRate { Percentage = 20 } };

            var price = variant.Price;

            price.Should().Be(1.2m);
        }

        [Fact]
        public void ProductVariant_Price_WithTaxesDisabledAndRateSetShouldBeTheSameAsPricePreTax()
        {
            _mockingKernel.SetTaxSettings(taxesEnabled: false);
            var variant = new ProductVariant { BasePrice = 1, TaxRate = new TaxRate { Percentage = 20 } };

            var price = variant.Price;

            price.Should().Be(1);
        }

        [Fact]
        public void ProductVariant_Price_ShouldBeRoundedTo2DecimalPlaces()
        {
            _mockingKernel.SetTaxSettings(true);
            var variant = new ProductVariant { BasePrice = 1, TaxRate = new TaxRate { Percentage = 17.5m } };

            var price = variant.Price;

            price.Should().Be(1.18m);
        }

        [Fact]
        public void ProductVariant_Tax_IfTaxesAreEnabledShouldBePriceMinusPricePreTax()
        {
            _mockingKernel.SetTaxSettings(true);
            var variant = new ProductVariant { BasePrice = 1, TaxRate = new TaxRate { Percentage = 17.5m } };

            var tax = variant.Tax;

            tax.Should().Be(0.18m);
        }

        [Fact]
        public void ProductVariant_Tax_IfTaxesAreDisabledShouldBeZero()
        {
            _mockingKernel.SetTaxSettings(false);
            var variant = new ProductVariant { BasePrice = 1, TaxRate = new TaxRate { Percentage = 17.5m } };

            var tax = variant.Tax;

            tax.Should().Be(0m);
        }

        [Theory]
        [PropertyData("TaxRates")]
        public void ProductVariant_TaxRatePercentage_TaxesEnabledIsTakenFromTheTaxRatesPercentage(TaxRate rate, decimal expected)
        {
            _mockingKernel.SetTaxSettings(true);
            var productVariant = new ProductVariant { TaxRate = rate };

            productVariant.TaxRatePercentage.Should().Be(expected);
        }

        [Theory]
        [PropertyData("TaxRates")]
        public void ProductVariant_TaxRatePercentage_TaxesDisabledIsZero(TaxRate rate, decimal expected)
        {
            _mockingKernel.SetTaxSettings(false);
            var productVariant = new ProductVariant { TaxRate = rate };

            productVariant.TaxRatePercentage.Should().Be(0);
        }

        [Fact]
        public void ProductVariant_TaxRatePercentage_IsZeroWhenTaxRateIsNull()
        {
            var productVariant = new ProductVariant { TaxRate = null };

            productVariant.TaxRatePercentage.Should().Be(0);
        }

        public static IEnumerable<object[]> TaxRates
        {
            get
            {
                yield return new object[] { new TaxRate { Percentage = 10 }, 10m };
                yield return new object[] { new TaxRate { Percentage = 20 }, 20m };
            }
        }


        [Fact]
        public void ProductVariant_PricePreTax_IfStoreSettingsAreNotLoadedPricesIncludeTaxShouldBeSameAsBasePrice()
        {
            _mockingKernel.SetTaxSettings(true, false);
            var product = new ProductVariant { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.PricePreTax.Should().Be(6);
        }

        [Fact]
        public void ProductVariant_PricePreTax_IfStoreSettingsAreLoadedPricesIncludeTaxShouldBeBasePriceLessVat()
        {
            _mockingKernel.SetTaxSettings(true, true);
            var product = new ProductVariant { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.PricePreTax.Should().Be(5);
        }

        [Fact]
        public void ProductVariant_Price_IfStoreSettingsAreNotLoadedPricesIncludeTaxShouldBeBasePricePlusVAT()
        {
            _mockingKernel.SetTaxSettings(true, false);
            var product = new ProductVariant { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.Price.Should().Be(7.2m);
        }

        [Fact]
        public void ProductVariant_Price_IfStoreSettingsAreLoadedPricesIncludeTaxShouldBeSameAsBasePrice()
        {
            _mockingKernel.SetTaxSettings(true, true);
            var product = new ProductVariant { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.Price.Should().Be(6);
        }

        [Fact]
        public void ProductVariant_GetUnitPrice_ShouldGetHighestQuantityMatchingPriceBreak()
        {
            _mockingKernel.SetTaxSettings(true, true);
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

        [Fact]
        public void ProductVariant_GetPrice_ShouldGetHighestQuantityMatchingPriceBreak()
        {
            _mockingKernel.SetTaxSettings(true, true);
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

            variant.GetPrice(3).Should().Be(24);
        }
    }
}