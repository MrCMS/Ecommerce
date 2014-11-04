using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class ShippingStatusTests
    {
        [Fact]
        public void IfShippingIsNotRequiredStatusShouldBeNotRequired()
        {
            var cartModel = new TestableCartModel(requiresShipping: false);

            cartModel.ShippingStatus.Should().Be(CartShippingStatus.ShippingNotRequired);
        }

        [Fact]
        public void IfShippingIsRequiredAndIsSetShouldBeShippingSet()
        {
            var cartModel = new TestableCartModel(requiresShipping: true){ShippingMethod = A.Fake<IShippingMethod>()};

            cartModel.ShippingStatus.Should().Be(CartShippingStatus.ShippingSet);
        }

        [Fact]
        public void IfShippingIsRequiredAndMethodNotSetAndNoPotentialMethodsShouldBeCannotShip()
        {
            var cartModel = new TestableCartModel(requiresShipping: true)
            {
                ShippingMethod = null,
                PotentiallyAvailableShippingMethods = new HashSet<IShippingMethod>()
            };

            cartModel.ShippingStatus.Should().Be(CartShippingStatus.CannotShip);
        }

        [Fact]
        public void IfShippingIsRequiredAndMethodNotSetAndSomePotentialMethodsShouldBeShippingNotSet()
        {
            var cartModel = new TestableCartModel(requiresShipping: true)
            {
                ShippingMethod = null,
                PotentiallyAvailableShippingMethods = new HashSet<IShippingMethod> { A.Fake<IShippingMethod>()}
            };

            cartModel.ShippingStatus.Should().Be(CartShippingStatus.ShippingNotSet);
        }
    }
}