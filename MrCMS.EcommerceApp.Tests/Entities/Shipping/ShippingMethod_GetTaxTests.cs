using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingMethod_GetTaxTests
    {
        [Fact]
        public void ReturnsNullIfNoneCanBeUsed()
        {
            var shippingMethod = new ShippingMethodBuilder().WithCanBeUsed(false).Build();

            var tax = shippingMethod.GetTax(new CartModel());

            tax.Should().Be(null);
        }

        [Fact]
        public void ReturnsTaxOfCalculationWithLowestPriceIfThereAreSeveral()
        {
            var shippingCalculation1 = new ShippingCalculationBuilder().WithTax(2).WithPrice(10).Build();
            var shippingCalculation2 = new ShippingCalculationBuilder().WithTax(1).WithPrice(20).Build();
            var shippingMethod = new ShippingMethodBuilder().WithCanBeUsed(true).WithOnlyTheCalculations(shippingCalculation1,shippingCalculation2).Build();

            var tax = shippingMethod.GetTax(new CartModel());

            tax.Should().Be(2);
        }
    }
}