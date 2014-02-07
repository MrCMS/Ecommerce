using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingCalculation_GetPriceTests : ShippingCalculationTests
    {
        [Fact]
        public void IfCannotBeUsedShouldBeNull()
        {
            var shippingCalculation = new ShippingCalculationBuilder().WithCanBeUsed(false).Build();

            var price = shippingCalculation.GetPrice(new CartModel());

            price.Should().Be(null);
        }

        [Fact]
        public void IfCanBeUsedShouldBePriceValue()
        {
            var shippingCalculation = new ShippingCalculationBuilder().WithCanBeUsed(true).WithPrice(1).Build();

            var price = shippingCalculation.GetPrice(new CartModel());

            price.Should().Be(1);
        }
    }
}