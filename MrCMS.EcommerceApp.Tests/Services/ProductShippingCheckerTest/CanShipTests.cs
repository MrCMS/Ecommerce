using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ProductShippingCheckerTest
{
    public class CanShipTests
    {
        [Fact]
        public void IfProductDoesNotHaveRestrictedShippingItCanShip()
        {
            var productVariant = new ProductVariantBuilder().WithUnrestrictedShipping().Build();
            var productShippingChecker = new ProductShippingCheckerBuilder().Build();

            productShippingChecker.CanShip(productVariant).Should().BeTrue();
        }

        [Fact]
        public void IfProductDoesHaveRestrictedShippingAndThereAreNoShippingTypesAvailableShouldNotShip()
        {
            var productVariant = new ProductVariantBuilder().WithRestrictedShipping().Build();
            var productShippingChecker = new ProductShippingCheckerBuilder().Build();

            productShippingChecker.CanShip(productVariant).Should().BeFalse();
        }

        [Fact]
        public void IfRestrictedShippingAndCartModelAndProductVariantHaveMatchingShippingMethodsCanShip()
        {
            var method = new StubShippingMethod1();
            var productVariant = new ProductVariantBuilder().WithRestrictedShipping(method.TypeName).Build();
            var cartModel = new CartModelBuilder().WithShippingOptions(method).Build();
            var productShippingChecker = new ProductShippingCheckerBuilder().WithCartModel(cartModel).Build();

            productShippingChecker.CanShip(productVariant).Should().BeTrue();
        }

        [Fact]
        public void IfRestrictedShippingAndCartModelAndProductVariantHaveNoSharedShippingMethodsCannotShip()
        {
            var method1 = new StubShippingMethod1();
            var method2 = new StubShippingMethod2();
            var productVariant = new ProductVariantBuilder().WithRestrictedShipping(method1.TypeName).Build();
            var cartModel = new CartModelBuilder().WithShippingOptions(method2).Build();
            var productShippingChecker = new ProductShippingCheckerBuilder().WithCartModel(cartModel).Build();

            productShippingChecker.CanShip(productVariant).Should().BeFalse();
        }
    }
}