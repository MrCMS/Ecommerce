using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.EcommerceApp.Tests.Services.BulkStockUpdate
{
    public class BulkStockUpdateServiceTests : InMemoryDatabaseTest
    {
        private readonly BulkStockUpdateService _bulkStockUpdateService;
        private readonly IProductVariantService _productVariantService;

        public BulkStockUpdateServiceTests()
        {
             _productVariantService = A.Fake<IProductVariantService>();
             _bulkStockUpdateService = new BulkStockUpdateService(_productVariantService);
        }

        [Fact]
        public void BulkStockUpdateService_BulkStockUpdateFromDTOs_ShouldReturnZeroIfNoItemsAreLinedUpForBulkStockUpdate()
        {
            var items = new List<BulkStockUpdateDataTransferObject>();

            var result = _bulkStockUpdateService.BulkStockUpdateFromDTOs(items);

            result.Should().Be(0);
        }

        [Fact]
        public void BulkStockUpdateService_BulkStockUpdateFromDTOs_ShouldCallGetAllOfProductVariantService()
        {
            var items = new List<BulkStockUpdateDataTransferObject>();

            _bulkStockUpdateService.BulkStockUpdateFromDTOs(items);

           A.CallTo(()=>_productVariantService.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void BulkStockUpdateService_BulkStockUpdate_ShouldReturnProductVariant()
        {
            var noOfUpdatedItems = 0;
            var item = new BulkStockUpdateDataTransferObject()
                {
                    Name="TP",
                    SKU="123",
                    StockRemaining = 10
                };

            var result = _bulkStockUpdateService.BulkStockUpdate(item, ref noOfUpdatedItems);

            result.Should().BeOfType<ProductVariant>();
        }

    }
}