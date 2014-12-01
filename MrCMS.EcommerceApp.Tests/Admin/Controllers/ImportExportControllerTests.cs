using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Areas.Admin.Helpers;
using Xunit;
using FluentAssertions;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ImportExportControllerTests : InMemoryDatabaseTest
    {
        private readonly IImportProductsManager _importExportManager;
        private readonly IExportProductsManager _exportProductsManager;
        private readonly ImportExportController _importExportController;

        public ImportExportControllerTests()
        {
            _exportProductsManager = A.Fake<IExportProductsManager>();
            _importExportManager = A.Fake<IImportProductsManager>();

            _importExportController = new ImportExportController(_importExportManager, _exportProductsManager);
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
        public void ImportExportController_ImportProducts_ShouldReturnRedirectToRouteResult()
        {
            var file = A.Fake<HttpPostedFileBase>();

            var result = _importExportController.ImportProducts(file);

            result.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ImportExportController_ImportProducts_ShouldCallImportProductsFromExcelOfImportExportManager()
        {
            var file = new BasicHttpPostedFileBase();
            _importExportController.ServerMock = A.Fake<HttpServerUtilityBase>();
            var value = Enumerable.Range(1,3).Select(i => i.ToString()).ToList();
            A.CallTo(() => _importExportManager.ImportProductsFromExcel(file.InputStream, true))
             .Returns(value);

            var result = _importExportController.ImportProducts(file);

            _importExportController.TempData.ErrorMessages().Should().BeEquivalentTo(value);
        }

    }
}