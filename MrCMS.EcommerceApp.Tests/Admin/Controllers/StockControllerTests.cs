using System;
using System.Web.Mvc;
using FakeItEasy;
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
            _productVariantService = A.Fake<ProductVariantService>();
            _inventoryService = A.Fake<IInventoryService>();
            _stockController=new StockController(_productVariantService,_inventoryService);
        }

        [Fact]
        public void StockController_LowStockReport_ReturnsViewResult()
        {
            var result = _stockController.LowStockReport(String.Empty,11);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void StockController_LowStockReport_ShouldCallGetAllVariantsWithLowStock()
        {
            _stockController.LowStockReport(String.Empty,11);

            A.CallTo(() => _productVariantService.GetAllVariantsWithLowStock(11)).MustHaveHappened();
        }

        [Fact]
        public void StockController_LowStockReport_ShouldSetTresholdInViewData()
        {
            var result=_stockController.LowStockReport(String.Empty, 11);

            result.ViewData["treshold"].Should().NotBeNull();
            result.ViewData["treshold"].Should().BeSameAs(11);
        }

        [Fact]
        public void StockController_LowStockReport_ShouldSetStatusIfExportingFailed()
        {
            var result = _stockController.LowStockReport("Exporting failed.", 11);

            ((object) result.ViewBag.Status).Should().NotBeNull();
        }

        [Fact]
        public void StockController_UpdateStock_ReturnsRedirectToRouteResult()
        {
            var pv = new ProductVariant(){Id=22};

            var result = _stockController.UpdateStock(pv, 11);

            result.Should().BeOfType<RedirectToRouteResult>();
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
            var pv = new ProductVariant() { Id = 22 };

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
        public void ImportExportController_ExportLowStockReport_ShouldCallExportProductsToGoogleBaseOfImportExportManager()
        {
            _stockController.ExportLowStockReport(11);

            A.CallTo(() => _inventoryService.ExportLowStockReport(11)).MustHaveHappened();
        }
    }
}