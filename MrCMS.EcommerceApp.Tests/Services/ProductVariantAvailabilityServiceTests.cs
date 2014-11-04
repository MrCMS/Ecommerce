using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ProductVariantAvailabilityServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void IfOutOfStockShouldReturnAnOutOfStockStatus()
        {
            ProductVariant productVariant = new ProductVariantBuilder().Build();
            ProductVariantAvailabilityService service = new ProductVariantAvailabilityServiceBuilder()
                .IsOutOfStock().Build();

            service.CanBuy(productVariant, 1).Should().BeOfType<OutOfStock>();
        }

        [Fact]
        public void IfInStockButCannotPurchaseTheRequestedAmountShouldReturnAnUnableToOrderQuantityStatus()
        {
            ProductVariant variant = new ProductVariantBuilder().Build();
            ProductVariantAvailabilityService service = new ProductVariantAvailabilityServiceBuilder().
                CannotOrderQuantity().Build();

            service.CanBuy(variant, 2).Should().BeOfType<CannotOrderQuantity>();
        }

        [Fact]
        public void IfCannotShipShouldReturnNoShippingMethodWouldBeAvailable()
        {
            ProductVariant variant = new ProductVariantBuilder().Build();
            ProductVariantAvailabilityService service =
                new ProductVariantAvailabilityServiceBuilder().CannotShip().Build();

            service.CanBuy(variant, 1).Should().BeOfType<NoShippingMethodWouldBeAvailable>();
        }

        [Fact]
        public void IfInStockAndCartItemAmountIsWithinStockRemainingShouldReturnCanBuy()
        {
            ProductVariant variant = new ProductVariantBuilder().Build();
            ProductVariantAvailabilityService service = new ProductVariantAvailabilityServiceBuilder().Build();

            service.CanBuy(variant, 2).Should().BeOfType<CanBuy>();
        }
    }
}