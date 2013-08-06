using System.IO;
using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.StockReport;
namespace MrCMS.EcommerceApp.Tests.Services
{
    public class InventoryServiceTests : InMemoryDatabaseTest
    {
        private readonly IStockReportService _stockReportService;
        private readonly IInventoryService _inventoryService;
        private readonly IBulkStockUpdateValidationService _bulkStockUpdateValidationService;
        private readonly IBulkStockUpdateService _bulkStockUpdateService;

        public InventoryServiceTests()
        {
            _bulkStockUpdateValidationService =  A.Fake<IBulkStockUpdateValidationService>(); 
            _bulkStockUpdateService =  A.Fake<IBulkStockUpdateService>();
            _stockReportService = A.Fake<IStockReportService>();
            _inventoryService = new InventoryService(_bulkStockUpdateValidationService, _bulkStockUpdateService,_stockReportService);
        }

        [Fact]
        public void InventoryService_ExportLowStockReport_ShouldReturnByteArray()
        {
            var result = _inventoryService.ExportLowStockReport(11);

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void InventoryService_ExportLowStockReport_ShouldCallGenerateLowStockReport()
        {
            _inventoryService.ExportLowStockReport(11);

            A.CallTo(() => _stockReportService.GenerateLowStockReport(11)).MustHaveHappened();
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
        public void InventoryService_ExportStockReport_ShouldCallGenerateStockReport()
        {
            _inventoryService.ExportStockReport();

            A.CallTo(() => _stockReportService.GenerateStockReport()).MustHaveHappened();
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

            result.Should().HaveCount(1);
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}