using System.IO;
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

        [Fact]
        public void InventoryService_ExportStockReport_ShouldReturnByteArray()
        {
            var result = _inventoryService.ExportStockReport();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void InventoryService_ExportStockReport_ShouldCallGetAllOfProductVariantService()
        {
            _inventoryService.ExportStockReport();

            A.CallTo(() => _productVariantService.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void InventoryService_ExportStockReport_ShouldNotBeNull()
        {
            var result = _inventoryService.ExportStockReport();

            result.Should().NotBeNull();
        }


        [Fact]
        public void InventoryService_BulkStockUpdate_ShouldNotBeNull()
        {
            var result = _inventoryService.BulkStockUpdate(GetDefaultStream());

            result.Should().NotBeNull();
        }

        [Fact]
        public void InventoryService_BulkStockUpdate_ShouldReturnDictionary()
        {
            var result = _inventoryService.BulkStockUpdate(GetDefaultStream());

            result.Should().HaveCount(0);
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}