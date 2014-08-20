using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class StockControllerTests : InMemoryDatabaseTest
    {
        private readonly IStockAdminService _stockAdminService;
        private readonly StockController _stockController;

        public StockControllerTests()
        {
            _stockAdminService = A.Fake<IStockAdminService>();
            _stockController = new StockController(_stockAdminService);
        }

        [Fact]
        public void StockController_LowStockReport_ReturnsViewResult()
        {
            ViewResult result = _stockController.LowStockReport(11);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void StockController_LowStockReportProductVariants_ReturnsPartialViewResult()
        {
            PartialViewResult result = _stockController.LowStockReportProductVariants(11, 1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void StockController_LowStockReportProductVariants_ShouldCallGetAllVariantsWithLowStock()
        {
            _stockController.LowStockReportProductVariants(11, 1);

            A.CallTo(() => _stockAdminService.GetAllVariantsWithLowStock(11, 1)).MustHaveHappened();
        }

        [Fact]
        public void ImportExportController_ExportLowStockReport_ShouldReturnFileContentResult()
        {
            ActionResult result = _stockController.ExportLowStockReport(11);

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void ImportExportController_ExportLowStockReport_ShouldCallExportLowStockReport()
        {
            _stockController.ExportLowStockReport(11);

            A.CallTo(() => _stockAdminService.ExportLowStockReport(11)).MustHaveHappened();
        }

        [Fact]
        public void StockController_BulkStockUpdate_ReturnsViewResult()
        {
            ViewResult result = _stockController.BulkStockUpdate();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void StockController_BulkStockUpdatePOST_ShouldRedirectToBulkStockUpdate()
        {
            var file = new BasicHttpPostedFileBaseCSV();

            RedirectToRouteResult result = _stockController.BulkStockUpdate_POST(file);

            result.RouteValues["action"].Should().Be("BulkStockUpdate");
        }

        [Fact]
        public void StockController_ExportStockReport_ReturnsFileContentResult()
        {
            ActionResult result = _stockController.ExportStockReport();

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void StockController_ExportStockReport_ShouldCallExportStockReport()
        {
            _stockController.ExportStockReport();

            A.CallTo(() => _stockAdminService.ExportStockReport()).MustHaveHappened();
        }
    }
}