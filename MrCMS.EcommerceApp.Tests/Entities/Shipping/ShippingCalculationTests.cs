using FakeItEasy;
using FluentAssertions;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingCalculationTests
    {
        private readonly MockingKernel _mockingKernel;

        public ShippingCalculationTests()
        {
            _mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(_mockingKernel);
        }

        private void SetTaxSettings(bool taxesEnabled = false, bool shippingRatesIncludeTax = false)
        {
            _mockingKernel.Bind<TaxSettings>().ToMethod(context => new TaxSettings
                                                                       {
                                                                           TaxesEnabled = taxesEnabled,
                                                                           ShippingRateTaxesEnabled = taxesEnabled,
                                                                           ShippingRateIncludesTax = shippingRatesIncludeTax
                                                                       });
        }

        [Fact]
        public void ShippingCalculation_AmountPreTax_ShouldBeBaseAmountIfShippingAmountsDoNotIncludeTaxAndTaxesAreEnabled()
        {
            SetTaxSettings(true, false);
            var shippingCalculation = new TestableShippingCalculation
            {
                OverrideTaxRate = new TaxRate { Percentage = 20 },
                BaseAmount = 12
            };
            shippingCalculation.AmountPreTax.Should().Be(12);
        }

        [Fact]
        public void ShippingCalculation_AmountPreTax_ShouldBeBaseAmountLessTaxIfShippingAmountsDoIncludeTaxAndTaxesAreEnabled()
        {
            SetTaxSettings(true, true);
            var shippingCalculation = new TestableShippingCalculation
            {
                OverrideTaxRate = new TaxRate { Percentage = 20 },
                BaseAmount = 12
            };
            shippingCalculation.AmountPreTax.Should().Be(10);
        }

        [Fact]
        public void ShippingCalculation_Amount_ShouldBeBaseAmountIfShippingAmountsDoIncludeTaxAndTaxesAreEnabled()
        {
            SetTaxSettings(true, true);
            var shippingCalculation = new TestableShippingCalculation
            {
                OverrideTaxRate = new TaxRate { Percentage = 20 },
                BaseAmount = 12
            };
            shippingCalculation.Amount.Should().Be(12);
        }

        [Fact]
        public void ShippingCalculation_Amount_ShouldBeBaseAmountPlusTaxIfShippingAmountsDoNotIncludeTax()
        {
            SetTaxSettings(true, false);
            var shippingCalculation = new TestableShippingCalculation
            {
                OverrideTaxRate = new TaxRate { Percentage = 20 },
                BaseAmount = 12
            };
            shippingCalculation.Amount.Should().Be(14.4m);
        }

        [Fact]
        public void ShippingCalculation_AmountPreTax_ShouldBeBaseAmountIfShippingTaxIsNotEnabled()
        {
            SetTaxSettings(false);
            var shippingCalculation = new TestableShippingCalculation
            {
                OverrideTaxRate = new TaxRate { Percentage = 20 },
                BaseAmount = 12
            };
            shippingCalculation.AmountPreTax.Should().Be(12);
        }

        [Fact]
        public void ShippingCalculation_Amount_ShouldBeBaseAmountIfShippingTaxIsNotEnabled()
        {
            SetTaxSettings(false);
            var shippingCalculation = new TestableShippingCalculation
            {
                OverrideTaxRate = new TaxRate { Percentage = 20 },
                BaseAmount = 12
            };
            shippingCalculation.Amount.Should().Be(12);
        }

        [Fact]
        public void ShippingCalculation_CanBeUsed_ShouldBeFalseIfGetPriceIsNull()
        {
            var shippingCalculation = new TestableShippingCalculation { GetPriceIsNull = true };
            var cartModel = new CartModel();

            var canBeUsed = shippingCalculation.CanBeUsed(cartModel);

            canBeUsed.Should().BeFalse();
        }

        [Fact]
        public void ShippingCalculation_CanBeUsed_ShouldBeTrueIfGetPriceIsSet()
        {
            var shippingCalculation = new TestableShippingCalculation { OverrideGetPrice = 1 };
            var cartModel = new CartModel();

            var canBeUsed = shippingCalculation.CanBeUsed(cartModel);

            canBeUsed.Should().BeTrue();
        }

        [Fact]
        public void ShippingCalculation_GetPrice_ShouldReturnValueIfIsByShippingWeightAndIsValid()
        {
            var shippingCalculation = new TestableShippingCalculation { OverridePrice = 1, ShippingCriteria = ShippingCriteria.ByWeight };
            var cartModel = A.Fake<CartModel>();
            A.CallTo(() => cartModel.Weight).Returns(10);

            var price = shippingCalculation.GetPrice(cartModel);

            price.Should().Be(1);
        }

        [Fact]
        public void ShippingCalculation_GetPrice_ShouldReturnNullIfIsByWeightAndIsBelowBounds()
        {
            var shippingCalculation = new TestableShippingCalculation
                                          {
                                              OverridePrice = 1,
                                              ShippingCriteria = ShippingCriteria.ByWeight,
                                              LowerBound = 20,
                                              UpperBound = 30
                                          };
            var cartModel = A.Fake<CartModel>();
            A.CallTo(() => cartModel.Weight).Returns(10);

            var price = shippingCalculation.GetPrice(cartModel);

            price.Should().Be(null);
        }

        [Fact]
        public void ShippingCalculation_GetPrice_ShouldReturnNullIfIsByWeightAndIsAboveBounds()
        {
            var shippingCalculation = new TestableShippingCalculation
            {
                OverridePrice = 1,
                ShippingCriteria = ShippingCriteria.ByWeight,
                LowerBound = 20,
                UpperBound = 30
            };
            var cartModel = A.Fake<CartModel>();
            A.CallTo(() => cartModel.Weight).Returns(40);

            var price = shippingCalculation.GetPrice(cartModel);

            price.Should().Be(null);
        }

        [Fact]
        public void ShippingCalculation_GetPrice_ShouldReturnValueIfIsByShippingTotalAndIsWithinBounds()
        {
            var shippingCalculation = new TestableShippingCalculation { OverridePrice = 1, ShippingCriteria = ShippingCriteria.ByCartTotal };
            var cartModel = A.Fake<CartModel>();
            A.CallTo(() => cartModel.Total).Returns(10);

            var price = shippingCalculation.GetPrice(cartModel);

            price.Should().Be(1);
        }

        [Fact]
        public void ShippingCalculation_GetPrice_ShouldReturnNullIfIsByShippingTotalAndIsBelowBounds()
        {
            var shippingCalculation = new TestableShippingCalculation
            {
                OverridePrice = 1,
                ShippingCriteria = ShippingCriteria.ByCartTotal,
                LowerBound = 20,
                UpperBound = 30
            };
            var cartModel = A.Fake<CartModel>();
            A.CallTo(() => cartModel.Total).Returns(10);

            var price = shippingCalculation.GetPrice(cartModel);

            price.Should().Be(null);
        }

        [Fact]
        public void ShippingCalculation_GetPrice_ShouldReturnNullIfIsByShippingTotalAndIsAboveBounds()
        {
            var shippingCalculation = new TestableShippingCalculation
            {
                OverridePrice = 1,
                ShippingCriteria = ShippingCriteria.ByCartTotal,
                LowerBound = 20,
                UpperBound = 30
            };
            var cartModel = A.Fake<CartModel>();
            A.CallTo(() => cartModel.Total).Returns(40);

            var price = shippingCalculation.GetPrice(cartModel);

            price.Should().Be(null);
        }

        [Fact]
        public void ShippingCalculation_Tax_ShouldBePriceMinusPricePreTax()
        {
            var shippingCalculation = new TestableShippingCalculation
                                          {
                                              OverridePrice = 25,
                                              OverridePricePreTax = 20
                                          };

            var tax = shippingCalculation.Tax;

            tax.Should().Be(5);
        }

        [Fact]
        public void ShippingCalculation_GetTax_IfGetPriceReturnsNullShouldReturnNull()
        {
            var shippingCalculation = new TestableShippingCalculation { GetPriceIsNull = true };

            var tax = shippingCalculation.GetTax(new CartModel());

            tax.Should().Be(null);
        }
    }

    [DoNotMap]
    public class TestableShippingCalculation : ShippingCalculation
    {
        public TaxRate OverrideTaxRate { get; set; }
        public override TaxRate TaxRate
        {
            get { return OverrideTaxRate ?? base.TaxRate; }
        }

        public bool GetPriceIsNull { get; set; }
        public decimal? OverrideGetPrice { get; set; }

        public override decimal? GetPrice(CartModel model)
        {
            if (GetPriceIsNull) return null;
            return OverrideGetPrice ?? base.GetPrice(model);
        }

        public decimal? OverridePrice { get; set; }
        public override decimal Amount
        {
            get { return OverridePrice ?? base.Amount; }
        }

        public decimal? OverridePricePreTax { get; set; }
        public override decimal AmountPreTax
        {
            get { return OverridePricePreTax ?? base.AmountPreTax; }
        }
    }
}