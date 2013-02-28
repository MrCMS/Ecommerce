using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Pages.ProductTests
{
    public class ProductStockLevelTests
    {
        [Fact]
        public void Product_InStock_ShouldBeTrueIfStockRemainingIsNull()
        {
            var product = new Product();

            var inStock = product.InStock;

            inStock.Should().BeTrue();
        }

        [Fact]
        public void Product_InStock_ShouldBeFalseIfStockRemainingIs0()
        {
            var product = new Product {StockRemaining = 0};

            var inStock = product.InStock;

            inStock.Should().BeFalse();
        }

        [Fact]
        public void Product_InStock_ShouldBeTrueIfStockRemainingIsGreaterThan0()
        {
            var product = new Product {StockRemaining = 1};

            var inStock = product.InStock;

            inStock.Should().BeTrue();
        }

        [Fact]
        public void Product_CanBuy_ReturnFalseForZeroAmount()
        {
            var product = new Product();

            var canBuy = product.CanBuy(0);

            canBuy.Should().BeFalse();
        }

        [Fact]
        public void Product_CanBuy_ReturnTrueForPositiveAmountWhenStockRemainingIsNull()
        {
            var product = new Product{StockRemaining = null};

            var canBuy = product.CanBuy(1);

            canBuy.Should().BeTrue();
        }

        [Fact]
        public void Product_CanBuy_ReturnFalseForPositiveAmountWhenStockRemainingIsZero()
        {
            var product = new Product{StockRemaining = 0};

            var canBuy = product.CanBuy(1);

            canBuy.Should().BeFalse();
        }

        [Fact]
        public void Product_CanBuy_ShouldReturnFalseWhenThereIsStockButLessThanRequested()
        {
            var product = new Product{StockRemaining = 1};

            var canBuy = product.CanBuy(2);

            canBuy.Should().BeFalse();
        }

        [Fact]
        public void Product_CanBuy_ShouldReturnTrueWhenThereIsMoreStockThanRequested()
        {
            var product = new Product{StockRemaining = 2};

            var canBuy = product.CanBuy(1);

            canBuy.Should().BeTrue();
        }
    }
}