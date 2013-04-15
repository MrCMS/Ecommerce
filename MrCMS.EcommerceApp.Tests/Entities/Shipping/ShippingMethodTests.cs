using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingMethodTests
    {
        [Fact]
        public void ShippingMethod_CanBeUsed_ReturnsFalseIfAllShippingCalculationsCannotBeUsed()
        {
            var shippingMethod = new ShippingMethod();
            var shippingCalculation = A.Fake<ShippingCalculation>();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation };
            A.CallTo(() => shippingCalculation.CanBeUsed(cartModel)).Returns(false);

            var canBeUsed = shippingMethod.CanBeUsed(cartModel);

            canBeUsed.Should().BeFalse();
        }

        [Fact]
        public void ShippingMethod_CanBeUsed_ReturnsTrueIfAnyShippingCalculationsCanBeUsed()
        {
            var shippingMethod = new ShippingMethod();
            var shippingCalculation = A.Fake<ShippingCalculation>();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation };
            A.CallTo(() => shippingCalculation.CanBeUsed(cartModel)).Returns(true);

            var canBeUsed = shippingMethod.CanBeUsed(cartModel);

            canBeUsed.Should().BeTrue();
        }

        [Fact]
        public void ShippingMethod_GetPrice_ReturnsNullIfNoneCanBeUsed()
        {
            var shippingMethod = new ShippingMethod();
            var shippingCalculation = A.Fake<ShippingCalculation>();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation };
            A.CallTo(() => shippingCalculation.CanBeUsed(cartModel)).Returns(false);

            var price = shippingMethod.GetPrice(cartModel);

            price.Should().Be(null);
        }

        [Fact]
        public void ShippingMethod_GetTax_ReturnsNullIfNoneCanBeUsed()
        {
            var shippingMethod = new ShippingMethod();
            var shippingCalculation = A.Fake<ShippingCalculation>();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation };
            A.CallTo(() => shippingCalculation.CanBeUsed(cartModel)).Returns(false);

            var tax = shippingMethod.GetTax(cartModel);

            tax.Should().Be(null);
        }

        [Fact]
        public void ShippingMethod_GetPrice_ReturnsLowestValueIfThereAreSeveral()
        {
            var shippingMethod = new ShippingMethod();
            var shippingCalculation1 = A.Fake<ShippingCalculation>();
            var shippingCalculation2 = A.Fake<ShippingCalculation>();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation1, shippingCalculation2 };
            A.CallTo(() => shippingCalculation1.CanBeUsed(cartModel)).Returns(true);
            A.CallTo(() => shippingCalculation2.CanBeUsed(cartModel)).Returns(true);
            A.CallTo(() => shippingCalculation1.GetPrice(cartModel)).Returns(20);
            A.CallTo(() => shippingCalculation2.GetPrice(cartModel)).Returns(10);

            var price = shippingMethod.GetPrice(cartModel);

            price.Should().Be(10);
        }

        [Fact]
        public void ShippingMethod_GetTax_ReturnsLowestValueIfThereAreSeveral()
        {
            var shippingMethod = new ShippingMethod();
            var shippingCalculation1 = A.Fake<ShippingCalculation>();
            var shippingCalculation2 = A.Fake<ShippingCalculation>();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation1, shippingCalculation2 };
            A.CallTo(() => shippingCalculation1.CanBeUsed(cartModel)).Returns(true);
            A.CallTo(() => shippingCalculation2.CanBeUsed(cartModel)).Returns(true);
            A.CallTo(() => shippingCalculation1.GetPrice(cartModel)).Returns(20);
            A.CallTo(() => shippingCalculation2.GetPrice(cartModel)).Returns(10);
            A.CallTo(() => shippingCalculation1.GetTax(cartModel)).Returns(2);
            A.CallTo(() => shippingCalculation2.GetTax(cartModel)).Returns(1);

            var tax = shippingMethod.GetTax(cartModel);

            tax.Should().Be(1);
        }
    }
}