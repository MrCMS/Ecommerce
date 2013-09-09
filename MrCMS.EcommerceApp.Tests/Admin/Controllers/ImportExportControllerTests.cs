using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Xunit;
using FluentAssertions;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ImportExportControllerTests : InMemoryDatabaseTest
    {
        private readonly IImportProductsManager _importExportManager;
        private readonly IExportProductsManager _exportProductsManager;
        private readonly IExportOrdersService _exportOrdersService;
        private readonly ImportExportController _importExportController;

        public ImportExportControllerTests()
        {
            _exportProductsManager = A.Fake<IExportProductsManager>();
            _exportOrdersService = A.Fake<IExportOrdersService>();
            _importExportManager = A.Fake<IImportProductsManager>();

            _importExportController = new ImportExportController(_importExportManager, _exportOrdersService, _exportProductsManager);
        }

        [Fact]
        public void ImportExportController_Products_ShouldReturnViewResult()
        {
            var result = _importExportController.Products();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ImportExportController_ExportProducts_ShouldReturnFileContentResult()
        {
            var result = _importExportController.ExportProducts();

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void ImportExportController_ImportProducts_ShouldReturnViewResult()
        {
            var file = A.Fake<HttpPostedFileBase>();

            var result = _importExportController.ImportProducts(file);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ImportExportController_ImportProducts_ShouldCallImportProductsFromExcelOfImportExportManager()
        {
            var file = new BasicHttpPostedFileBase();

            A.CallTo(() => _importExportManager.ImportProductsFromExcel(file.InputStream))
             .Returns(new Dictionary<string, List<string>>());
    
            var result = _importExportController.ImportProducts(file);

            AssertionExtensions.Should((object)AssertionExtensions.As<Dictionary<string, List<string>>>(result.ViewBag.Messages)).NotBeNull();
        }

    }
}