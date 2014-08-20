using System.IO;
using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.StockReport;
namespace MrCMS.EcommerceApp.Tests.Services
{
    public class StockAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly IStockReportService _stockReportService;
        private readonly IStockAdminService _stockAdminService;
        private readonly IBulkStockUpdateValidationService _bulkStockUpdateValidationService;
        private readonly IBulkStockUpdateService _bulkStockUpdateService;

        public StockAdminServiceTests()
        {
            _bulkStockUpdateValidationService =  A.Fake<IBulkStockUpdateValidationService>(); 
            _bulkStockUpdateService =  A.Fake<IBulkStockUpdateService>();
            _stockReportService = A.Fake<IStockReportService>();
            _stockAdminService = new StockAdminService(_bulkStockUpdateValidationService, _bulkStockUpdateService,
                _stockReportService, Session);
        }

        [Fact]
        public void ExportLowStockReport_ShouldReturnByteArray()
        {
            var result = _stockAdminService.ExportLowStockReport(11);

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ExportLowStockReport_ShouldCallGenerateLowStockReport()
        {
            _stockAdminService.ExportLowStockReport(11);

            A.CallTo(() => _stockReportService.GenerateLowStockReport(11)).MustHaveHappened();
        }

        [Fact]
        public void ExportLowStockReport_ShouldNotBeNull()
        {
            var result = _stockAdminService.ExportLowStockReport(11);

            result.Should().NotBeNull();
        }

        [Fact]
        public void ExportStockReport_ShouldReturnByteArray()
        {
            var result = _stockAdminService.ExportStockReport();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ExportStockReport_ShouldCallGenerateStockReport()
        {
            _stockAdminService.ExportStockReport();

            A.CallTo(() => _stockReportService.GenerateStockReport()).MustHaveHappened();
        }

        [Fact]
        public void ExportStockReport_ShouldNotBeNull()
        {
            var result = _stockAdminService.ExportStockReport();

            result.Should().NotBeNull();
        }


        [Fact]
        public void BulkStockUpdate_ShouldNotBeNull()
        {
            var result = _stockAdminService.BulkStockUpdate(GetDefaultStream());

            result.Should().NotBeNull();
        }

        [Fact]
        public void BulkStockUpdate_ShouldReturnDictionary()
        {
            var result = _stockAdminService.BulkStockUpdate(GetDefaultStream());

            result.Should().HaveCount(1);
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}