using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Products.ProductVariantTests
{
    public class ProductVariant_CanBuyTests
    {
        [Fact]
        public void IfOutOfStockShouldReturnAnOutOfStockStatus()
        {
            var productVariant = new ProductVariantBuilder().IsOutOfStock().Build();

            productVariant.CanBuy(new CartModel()).Should().BeOfType<OutOfStock>();
        }

        [Fact]
        public void IfInStockButCannotPurchaseTheCartItemAmountShouldReturnAnUnableToOrderQuantityStatus()
        {
            var variant = new ProductVariantBuilder().WithStockRemaining(1).Build();
            var item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            var cartModel = new CartModelBuilder().WithItems(item).Build();

            variant.CanBuy(cartModel).Should().BeOfType<CannotOrderQuantity>();
        }

        [Fact]
        public void IfAdditionalQuantityTakesAmountRequestedOverStockRemainingShouldReturnCannotOrderQuantity()
        {
            var variant = new ProductVariantBuilder().WithStockRemaining(2).Build();
            var item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            var cartModel = new CartModelBuilder().WithItems(item).Build();

            variant.CanBuy(cartModel, 1).Should().BeOfType<CannotOrderQuantity>();
        }

        [Fact]
        public void IfOnlyAvailableShippingMethodIsExcludedShouldReturnNoShippingMethodIsAvailable()
        {
            var variant = new ProductVariantBuilder().WithStockRemaining(2).Build();
            var item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            var cartModel = new CartModelBuilder().WithItems(item).Build();
            variant.RestrictedShippingMethods = cartModel.AvailableShippingMethods;

            variant.CanBuy(cartModel).Should().BeOfType<NoShippingMethodWouldBeAvailable>();
        }

        [Fact]
        public void IfInStockAndCartItemAmountIsWithinStockRemainingShouldReturnCanBuy()
        {
            var variant = new ProductVariantBuilder().WithStockRemaining(2).Build();
            var item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            var cartModel = new CartModelBuilder().WithItems(item).Build();

            variant.CanBuy(cartModel).Should().BeOfType<CanBuy>();
        }
    }
}