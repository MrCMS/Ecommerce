using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingCalculation_GetTaxTests
    {
        [Fact]
        public void IfCannotBeUsedShouldBeNull()
        {
            var shippingCalculation = new ShippingCalculationBuilder().WithCanBeUsed(false).Build();

            var tax = shippingCalculation.GetTax(new CartModel());

            tax.Should().Be(null);
        }
        [Fact]
        public void IfCanBeUsedShouldBeTaxValue()
        {
            var shippingCalculation = new ShippingCalculationBuilder().WithCanBeUsed(true).WithTax(1).Build();

            var tax = shippingCalculation.GetTax(new CartModel());

            tax.Should().Be(1m);
        }
    }
}