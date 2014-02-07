using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingMethod_GetPriceTests
    {
        [Fact]
        public void ReturnsNullIfMethodCannotBeUsed()
        {
            var shippingMethod = new ShippingMethodBuilder().WithCanBeUsed(false).Build();

            var price = shippingMethod.GetPrice(new CartModel());

            price.Should().Be(null);
        }

        [Fact]
        public void ReturnsLowestValueIfThereAreSeveralValid()
        {
            var shippingCalculation1 = new ShippingCalculationBuilder().WithCanBeUsed(true).WithPrice(20).Build();
            var shippingCalculation2 = new ShippingCalculationBuilder().WithCanBeUsed(true).WithPrice(10).Build();
            var shippingMethod = new ShippingMethodBuilder().WithOnlyTheCalculations(shippingCalculation1, shippingCalculation2).Build();
            var cartModel = new CartModel();

            var price = shippingMethod.GetPrice(cartModel);

            price.Should().Be(10);
        }

        [Fact]
        public void DoesNotReturnALowerPriceIfItCannotBeUsed()
        {
            var shippingCalculation1 = new ShippingCalculationBuilder().WithCanBeUsed(true).WithPrice(20).Build();
            var shippingCalculation2 = new ShippingCalculationBuilder().WithCanBeUsed(false).WithPrice(10).Build();
            var shippingMethod = new ShippingMethodBuilder().WithOnlyTheCalculations(shippingCalculation1, shippingCalculation2).Build();
            var cartModel = new CartModel();

            var price = shippingMethod.GetPrice(cartModel);

            price.Should().Be(20);
        }
    }
}