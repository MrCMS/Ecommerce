using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using FluentAssertions;
namespace MrCMS.EcommerceApp.Tests.Services
{
    public class InventoryServiceTests : InMemoryDatabaseTest
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IInventoryService _inventoryService;
        private readonly IBulkStockUpdateValidationService _bulkStockUpdateValidationService;
        private readonly IBulkStockUpdateService _bulkStockUpdateService;

        public InventoryServiceTests()
        {
            _bulkStockUpdateValidationService =  A.Fake<IBulkStockUpdateValidationService>();
            _bulkStockUpdateService =  A.Fake<IBulkStockUpdateService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _inventoryService = new InventoryService(_bulkStockUpdateValidationService, _bulkStockUpdateService,A.Fake<ProductVariantService>());
        }

        [Fact]
        public void InventoryService_ExportLowStockReport_ShouldReturnByteArray()
        {
            var result = _inventoryService.ExportLowStockReport(11);

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void InventoryService_ExportLowStockReport_ShouldCallGetAllOfProductVariantService()
        {
            _inventoryService.ExportLowStockReport(11);

            A.CallTo(() => _productVariantService.GetAllVariantsWithLowStock(11)).MustHaveHappened();
        }

        [Fact]
        public void InventoryService_ExportLowStockReport_ShouldNotBeNull()
        {
            var result = _inventoryService.ExportLowStockReport(11);

            result.Should().NotBeNull();
        }
    }
}