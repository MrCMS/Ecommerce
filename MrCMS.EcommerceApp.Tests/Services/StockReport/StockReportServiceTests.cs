using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.StockReport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.StockReport
{
    public class StockReportServiceTests : InMemoryDatabaseTest
    {
        private readonly StockReportService _stockReportService;
        private readonly IProductVariantService _productVariantService;

        public StockReportServiceTests()
        {
             _productVariantService = A.Fake<IProductVariantService>();
             _stockReportService = new StockReportService(_productVariantService);
        }

        [Fact]
        public void StockReportService_GenerateLowStockReport_ShouldReturnByteArray()
        {
            var result = _stockReportService.GenerateLowStockReport(11);

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void StockReportService_GenerateLowStockReport_ShouldCallGetAllVariantsWithLowStock()
        {
            _stockReportService.GenerateLowStockReport(11);

            A.CallTo(() => _productVariantService.GetAllVariantsWithLowStock(11)).MustHaveHappened();
        }

        [Fact]
        public void StockReportService_GenerateStockReport_ShouldReturnByteArray()
        {
            var result = _stockReportService.GenerateStockReport();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void StockReportService_GenerateLowStockReport_ShouldCallGetAllOfProductVariantService()
        {
            _stockReportService.GenerateStockReport();

            A.CallTo(() => _productVariantService.GetAllVariantsForStockReport()).MustHaveHappened();
        }
    }
}