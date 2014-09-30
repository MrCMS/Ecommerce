using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
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

        public class SimpleStockManagementTests
        {
            [Fact]
            public void IfStockIsTrackedAVariantWithZeroRemainingStockShouldNotBeInStock()
            {
                ProductVariant productVariant = new ProductVariantBuilder().StockRemaining(0).Build();
                ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().Build();

                productStockChecker.IsInStock(productVariant).Should().BeFalse();
            }

            [Fact]
            public void IfStockIsTrackedAVariantWithSomeStockRemainingStockShouldBeInStock()
            {
                ProductVariant productVariant = new ProductVariantBuilder().StockRemaining(1).Build();
                ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().Build();

                productStockChecker.IsInStock(productVariant).Should().BeTrue();
            }
        }

        public class WarehousedStockManagementTests : InMemoryDatabaseTest
        {
            [Fact]
            public void IfNoStockLevelsExistTheVariantShouldNotBeInStock()
            {
                ProductVariant productVariant = new ProductVariantBuilder().BuildAndPersist(Session);
                ProductStockChecker productStockChecker =
                    new ProductStockCheckerBuilder().WithSession(Session).WithWarehousesEnabled().Build();

                productStockChecker.IsInStock(productVariant).Should().BeFalse();
            }

            [Fact]
            public void IfAStockLevelExistsAndHasAValueShouldBeInStock()
            {
                ProductVariant productVariant = new ProductVariantBuilder().BuildAndPersist(Session);
                Warehouse warehouse = new WarehouseBuilder().WithName("Test Warehouse").BuildAndPersist(Session);
                WarehouseStock warehouseStock =
                    new WarehouseStockBuilder(productVariant, warehouse).WithStockLevel(1).BuildAndPersist(Session);
                ProductStockChecker productStockChecker =
                    new ProductStockCheckerBuilder().WithSession(Session).WithWarehousesEnabled().Build();

                productStockChecker.IsInStock(productVariant).Should().BeTrue();
            }
        }
    }
}