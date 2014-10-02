using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.EcommerceApp.Tests.Services.ProductStockCheckerTest
{
    public class CanOrderQuantityTests
    {
        [Fact]
        public void UntrackedVariantsAreAvailableInAnyQuantity()
        {
            ProductVariant productVariant = new ProductVariantBuilder().DoNotTrackStock().Build();
            ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().Build();

            productStockChecker.CanOrderQuantity(productVariant, 999).CanOrder.Should().BeTrue();
        }

        [Fact]
        public void IfStockIsTrackedAVariantWithLessThanTheRequestedAmountShouldNotBeAvailable()
        {
            ProductVariant productVariant = new ProductVariantBuilder().StockRemaining(1).Build();
            ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().Build();

            productStockChecker.CanOrderQuantity(productVariant, 2).CanOrder.Should().BeFalse();
        }

        [Fact]
        public void IfTheStockLevelIsBelowTheRequestedAmountItShouldBeReturned()
        {
            ProductVariant productVariant = new ProductVariantBuilder().Build();
            ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().StockRemaining(1).Build();

            productStockChecker.CanOrderQuantity(productVariant, 2).StockRemaining.Should().Be(1);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void IfStockIsTrackedAVariantWithEqualOrMoreThanTheRequestedAmountShouldBeAvailable(int remaining)
        {
            ProductVariant productVariant = new ProductVariantBuilder().Build();
            ProductStockChecker productStockChecker = new ProductStockCheckerBuilder().StockRemaining(remaining).Build();

            productStockChecker.CanOrderQuantity(productVariant, 2).CanOrder.Should().BeTrue();
        }
        //public class SimpleStockManagementTests
        //{
        //}

        //public class WarehousedStockManagementTests : InMemoryDatabaseTest
        //{
        //    [Fact]
        //    public void IfTheStockLevelIsBelowTheRequestedAmountItShouldBeUnavailable()
        //    {
        //        ProductVariant productVariant = new ProductVariantBuilder().BuildAndPersist(Session);
        //        Warehouse warehouse = new WarehouseBuilder().WithName("Test Warehouse").BuildAndPersist(Session);
        //        WarehouseStock warehouseStock =
        //            new WarehouseStockBuilder(productVariant, warehouse).WithStockLevel(1).BuildAndPersist(Session);
        //        ProductStockChecker productStockChecker =
        //            new ProductStockCheckerBuilder().WithSession(Session).WithWarehousesEnabled().Build();

        //        productStockChecker.CanOrderQuantity(productVariant, 2).CanOrder.Should().BeFalse();
        //    }

        //    [Fact]
        //    public void IfTheStockLevelIsBelowTheRequestedAmountItShouldBeReturned()
        //    {
        //        ProductVariant productVariant = new ProductVariantBuilder().BuildAndPersist(Session);
        //        Warehouse warehouse = new WarehouseBuilder().WithName("Test Warehouse").BuildAndPersist(Session);
        //        WarehouseStock warehouseStock =
        //            new WarehouseStockBuilder(productVariant, warehouse).WithStockLevel(1).BuildAndPersist(Session);
        //        ProductStockChecker productStockChecker =
        //            new ProductStockCheckerBuilder().WithSession(Session).WithWarehousesEnabled().Build();

        //        productStockChecker.CanOrderQuantity(productVariant, 2).StockRemaining.Should().Be(1);
        //    }

        //    [Theory]
        //    [InlineData(2)]
        //    [InlineData(3)]
        //    public void IfTheStockLevelIsEqualToOrAboveTheRequestedAmountItShouldBeAvailable(int stockLevel)
        //    {
        //        ProductVariant productVariant = new ProductVariantBuilder().BuildAndPersist(Session);
        //        Warehouse warehouse = new WarehouseBuilder().WithName("Test Warehouse").BuildAndPersist(Session);
        //        WarehouseStock warehouseStock =
        //            new WarehouseStockBuilder(productVariant, warehouse).WithStockLevel(stockLevel)
        //                .BuildAndPersist(Session);
        //        ProductStockChecker productStockChecker =
        //            new ProductStockCheckerBuilder().WithSession(Session).WithWarehousesEnabled().Build();

        //        productStockChecker.CanOrderQuantity(productVariant, 2).CanOrder.Should().BeTrue();
        //    }
        //}
    }
}