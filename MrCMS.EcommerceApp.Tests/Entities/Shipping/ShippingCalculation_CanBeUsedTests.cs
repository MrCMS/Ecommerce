using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;
using Xunit.Extensions;
using MrCMS.EcommerceApp.Tests.Helpers;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingCalculation_CanBeUsedTests
    {
        [Theory]
        [InlineData(0, 0, null, true)]
        [InlineData(100, 0, null, true)]
        [InlineData(0, 10, null, false)]
        [InlineData(40, 10, 20D, false)]
        [InlineData(15, 10, 20D, true)]
        public void ShouldBeCalculatedCorrectlyWhenBasedOnWeight(double weight, double lowerBound, double? upperBound,
                                                                 bool result)
        {
            var shippingCalculation =
                new ShippingCalculationBuilder().WithLowerBound(lowerBound.ToDecimal())
                                                .WithUpperBound(upperBound.ToDecimal())
                                                .WithShippingCriteria(ShippingCriteria.ByWeight)
                                                .Build();
            var cartModel = new CartModelBuilder().WithWeight(weight.ToDecimal()).Build();

            shippingCalculation.CanBeUsed(cartModel).Should().Be(result);
        }

        [Theory]
        [InlineData(0, 0, null, true)]
        [InlineData(100, 0, null, true)]
        [InlineData(0, 10, null, false)]
        [InlineData(40, 10, 20D, false)]
        [InlineData(15, 10, 20D, true)]
        public void ShouldBeCalculatedCorrectlyWhenBasedOnTotal(double total, double lowerBound, double? upperBound,
                                                                bool result)
        {
            var shippingCalculation =
                new ShippingCalculationBuilder().WithLowerBound(lowerBound.ToDecimal())
                                                .WithUpperBound(upperBound.ToDecimal())
                                                .WithShippingCriteria(ShippingCriteria.ByCartTotal)
                                                .Build();
            var cartModel = new CartModelBuilder().WithTotalPreShipping(total.ToDecimal()).Build();

            shippingCalculation.CanBeUsed(cartModel).Should().Be(result);
        }

        [Fact]
        public void IfCountryIsSetAndShippingAddressCountryIsNullShouldBeFalse()
        {
            var country = new Country();
            var calculation = new ShippingCalculationBuilder().WithCountry(country).Build();
            var cartModel = new CartModelBuilder().WithShippingAddressCountry(null).Build();

            calculation.CanBeUsed(cartModel).Should().BeFalse();
        }

        [Fact]
        public void IfCountryIsSetAndShippingAddressCountryIsDifferentShouldBeFalse()
        {
            var country = new Country();
            var calculation = new ShippingCalculationBuilder().WithCountry(country).Build();
            var cartModel = new CartModelBuilder().WithShippingAddressCountry(new Country()).Build();

            calculation.CanBeUsed(cartModel).Should().BeFalse();
        }

        [Fact]
        public void IfCountryIsSetAndShippingAddressCountryIsSameShouldBeTrue()
        {
            var country = new Country();
            var calculation = new ShippingCalculationBuilder().WithCountry(country).Build();
            var cartModel = new CartModelBuilder().WithShippingAddressCountry(country).Build();

            calculation.CanBeUsed(cartModel).Should().BeTrue();
        }

        [Fact]
        public void IfOneOfTheProductsInTheCartIsExcludedItCannotBeUsed()
        {
            var productVariant = new ProductVariant();
            var shippingCalculation = new ShippingCalculationBuilder().WithRestrictedVariant(productVariant).Build();
            var cartModel = new CartModel
            {
                Items = new List<CartItem> { new CartItem { Item = productVariant } }
            };

            shippingCalculation.CanBeUsed(cartModel).Should().BeFalse();
        }

        [Fact]
        public void IfTheShippingMethodIsNullIsCannotBeUsed()
        {
            var shippingCalculation = new ShippingCalculationBuilder().Build();
            shippingCalculation.ShippingMethod = null;

            shippingCalculation.CanBeUsed(new CartModel()).Should().BeFalse();
        }
    }
}