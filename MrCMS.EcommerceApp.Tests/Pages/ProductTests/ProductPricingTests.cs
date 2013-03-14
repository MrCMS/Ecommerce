using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.EcommerceApp.Tests.Pages.ProductTests
{
    public class ProductPricingTests
    {
        private MockingKernel _mockingKernel;

        public ProductPricingTests()
        {
            _mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(_mockingKernel);
        }

        [Fact]
        public void Product_ReducedBy_IsZeroIfPreviousPriceIsNotSet()
        {
            var product = new Product { BasePrice = 1, PreviousPrice = null };

            var reducedPrice = product.ReducedBy;

            reducedPrice.Should().Be(0);
        }

        [Fact]
        public void Product_ReducedBy_ShouldBeTheDifferenceIfThePreviousPriceIsGreaterThanThePrice()
        {
            var product = new Product { BasePrice = 1, PreviousPrice = 2 };

            var reducedPrice = product.ReducedBy;

            reducedPrice.Should().Be(1);
        }

        [Fact]
        public void Product_ReducedBy_ShouldBeZeroIfPriceIsGreaterThanPreviousPrice()
        {
            var product = new Product { BasePrice = 2, PreviousPrice = 1 };

            var reducedPrice = product.ReducedBy;

            reducedPrice.Should().Be(0);
        }

        [Fact]
        public void Product_ReducedByPercentage_ShouldBeTheReducedByAsAPercentageOfThePreviousPrice()
        {
            var product = new Product { BasePrice = 1, PreviousPrice = 2 };

            var reducedByPercentage = product.ReducedByPercentage;

            reducedByPercentage.Should().Be(0.5m);
        }

        [Fact]
        public void Product_ReducedByPercentage_ShouldReturnZeroIfPreviousPriceIsNull()
        {
            var product = new Product { BasePrice = 1 };

            var reducedByPercentage = product.ReducedByPercentage;

            reducedByPercentage.Should().Be(0);
        }

        [Fact]
        public void Product_Price_WithNoTaxRateShouldBePricePreTax()
        {
            var product = new Product { BasePrice = 1 };

            var price = product.Price;

            price.Should().Be(1);
        }

        [Fact]
        public void Product_Price_WithTaxRateSetShouldBeTheSameAsPricePreTax()
        {
            var product = new Product { BasePrice = 1, TaxRate = new TaxRate { Percentage = 20 } };

            var price = product.Price;

            price.Should().Be(1.2m);
        }

        [Fact]
        public void Product_Price_ShouldBeRoundedTo2DecimalPlaces()
        {
            var product = new Product { BasePrice = 1, TaxRate = new TaxRate { Percentage = 17.5m } };

            var price = product.Price;

            price.Should().Be(1.18m);
        }

        [Fact]
        public void Product_Tax_ShouldBePriceMinusPricePreTax()
        {
            var product = new Product { BasePrice = 1, TaxRate = new TaxRate { Percentage = 17.5m } };

            var tax = product.Tax;

            tax.Should().Be(0.18m);
        }

        [Theory]
        [PropertyData("TaxRates")]
        public void Product_TaxRatePercentage_IsTakenFromTheTaxRatesPercentage(TaxRate rate, decimal expected)
        {
            var product = new Product { TaxRate = rate };

            product.TaxRatePercentage.Should().Be(expected);
        }

        [Fact]
        public void Product_TaxRatePercentage_IsZeroWhenTaxRateIsNull()
        {
            var product = new Product { TaxRate = null };

            product.TaxRatePercentage.Should().Be(0);
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
        public void Product_PricePreTax_IfStoreSettingsAreNotLoadedPricesIncludeTaxShouldBeSameAsBasePrice()
        {
            _mockingKernel.Bind<StoreSettings>().ToMethod(context => new StoreSettings { LoadedPricesIncludeTax = false });
            var product = new Product { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.PricePreTax.Should().Be(6);
        }

        [Fact]
        public void Product_PricePreTax_IfStoreSettingsAreLoadedPricesIncludeTaxShouldBeBasePriceLessVat()
        {
            _mockingKernel.Bind<StoreSettings>().ToMethod(context => new StoreSettings { LoadedPricesIncludeTax = true });
            var product = new Product { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.PricePreTax.Should().Be(5);
        }

        [Fact]
        public void Product_Price_IfStoreSettingsAreNotLoadedPricesIncludeTaxShouldBeBasePricePlusVAT()
        {
            _mockingKernel.Bind<StoreSettings>().ToMethod(context => new StoreSettings { LoadedPricesIncludeTax = false });
            var product = new Product { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.Price.Should().Be(7.2m);
        }

        [Fact]
        public void Product_Price_IfStoreSettingsAreLoadedPricesIncludeTaxShouldBeSameAsBasePrice()
        {
            _mockingKernel.Bind<StoreSettings>().ToMethod(context => new StoreSettings { LoadedPricesIncludeTax = true });
            var product = new Product { BasePrice = 6, TaxRate = new TaxRate { Percentage = 20 } };

            product.Price.Should().Be(6);
        }
    }
}