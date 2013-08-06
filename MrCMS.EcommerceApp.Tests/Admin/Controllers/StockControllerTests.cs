using System;
using System.IO;
using System.Web.Mvc;
using FakeItEasy;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class StockControllerTests : InMemoryDatabaseTest
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IInventoryService _inventoryService;
        private readonly StockController _stockController;

        public StockControllerTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();
            _inventoryService = A.Fake<IInventoryService>();
            _stockController=new StockController(_productVariantService,_inventoryService);
        }

        [Fact]
        public void StockController_LowStockReport_ReturnsViewResult()
        {
            var result = _stockController.LowStockReport(11);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void StockController_LowStockReportProductVariants_ReturnsPartialViewResult()
        {
            var result = _stockController.LowStockReportProductVariants(11,1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void StockController_LowStockReportProductVariants_ShouldCallGetAllVariantsWithLowStock()
        {
            _stockController.LowStockReportProductVariants(11, 1);

            A.CallTo(() => _productVariantService.GetAllVariantsWithLowStock(11,1)).MustHaveHappened();
        }

        [Fact]
        public void StockController_UpdateStock_ReturnsJsonResult()
        {
            var pv = new ProductVariant(){Id=22};

            var result = _stockController.UpdateStock(pv, 11);

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void StockController_UpdateStock_ShouldCallGetOfProductVariantService()
        {
            var pv = new ProductVariant() { Id = 22, StockRemaining = 10};

            _stockController.UpdateStock(pv, 11);

            A.CallTo(() => _productVariantService.Get(22)).MustHaveHappened();
        }

        [Fact]
        public void StockController_UpdateStock_ShouldCallUpdateOfProductVariantService()
        {
            var pv = new ProductVariant() { Id = 22, StockRemaining = 11};

            A.CallTo(() => _productVariantService.Get(22)).Returns(pv);

            _stockController.UpdateStock(pv, 11);

            A.CallTo(() => _productVariantService.Update(pv)).MustHaveHappened();
        }

        [Fact]
        public void ImportExportController_ExportLowStockReport_ShouldReturnFileContentResult()
        {
            var result = _stockController.ExportLowStockReport(11);

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void ImportExportController_ExportLowStockReport_ShouldCallExportLowStockReport()
        {
            _stockController.ExportLowStockReport(11);

            A.CallTo(() => _inventoryService.ExportLowStockReport(11)).MustHaveHappened();
        }

        [Fact]
        public void StockController_BulkStockUpdate_ReturnsViewResult()
        {
            var result = _stockController.BulkStockUpdate();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void StockController_BulkStockUpdatePOST_ShouldRedirectToBulkStockUpdate()
        {
            var file = new BasicHttpPostedFileBaseCSV();

            var result=_stockController.BulkStockUpdate_POST(file);

            result.RouteValues["action"].Should().Be("BulkStockUpdate");
        }

        [Fact]
        public void StockController_ExportStockReport_ReturnsFileContentResult()
        {
            var result = _stockController.ExportStockReport();

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void StockController_ExportStockReport_ShouldCallExportStockReport()
        {
            _stockController.ExportStockReport();

            A.CallTo(() => _inventoryService.ExportStockReport()).MustHaveHappened();
        }
    }
}