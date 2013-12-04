using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public class ShippingMethod_CanBeUsedTests
    {
        [Fact]
        public void ReturnsFalseIfAllShippingCalculationsCannotBeUsed()
        {
            var shippingCalculation = new ShippingCalculationBuilder().WithCanBeUsed(false).Build();
            var shippingMethod = new ShippingMethodBuilder().WithOnlyTheCalculations(shippingCalculation).Build();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation };

            var canBeUsed = shippingMethod.CanBeUsed(cartModel);

            canBeUsed.Should().BeFalse();
        }

        [Fact]
        public void ReturnsTrueIfAnyShippingCalculationsCanBeUsed()
        {
            var shippingCalculation = new ShippingCalculationBuilder().WithCanBeUsed(true).Build();
            var shippingMethod = new ShippingMethodBuilder().WithOnlyTheCalculations(shippingCalculation).Build();
            var cartModel = new CartModel();
            shippingMethod.ShippingCalculations = new List<ShippingCalculation> { shippingCalculation };

            var canBeUsed = shippingMethod.CanBeUsed(cartModel);

            canBeUsed.Should().BeTrue();
        }

        [Fact]
        public void IfOneOfTheProductsInTheCartIsExcludedItCannotBeUsed()
        {
            var productVariant = new ProductVariant();
            var shippingMethod = new ShippingMethodBuilder().WithRestrictedVariant(productVariant).Build();
            var cartModel = new CartModel
                                {
                                    Items = new List<CartItem> { new CartItem { Item = productVariant } }
                                };

            shippingMethod.CanBeUsed(cartModel).Should().BeFalse();
        }
    }
}