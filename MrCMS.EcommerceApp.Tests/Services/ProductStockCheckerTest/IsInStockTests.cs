using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ProductStockCheckerTest
{
    public class IsInStockTests
    {
        [Fact]
        public void UntrackedVariantsAreAlwaysInStock()
        {
            ProductVariant productVariant = new ProductVariantBuilder().DoNotTrackStock().Build();
            ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().Build();

            productStockChecker.IsInStock(productVariant).Should().BeTrue();
        }

        [Fact]
        public void IfStockIsTrackedAVariantWithZeroRemainingStockShouldNotBeInStock()
        {
            ProductVariant productVariant = new ProductVariantBuilder().Build();
            ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().StockRemaining(0).Build();

            productStockChecker.IsInStock(productVariant).Should().BeFalse();
        }

        [Fact]
        public void IfStockIsTrackedAVariantWithSomeStockRemainingStockShouldBeInStock()
        {
            ProductVariant productVariant = new ProductVariantBuilder().StockRemaining(1).Build();
            ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().StockRemaining(1).Build();

            productStockChecker.IsInStock(productVariant).Should().BeTrue();
        }
    }
}