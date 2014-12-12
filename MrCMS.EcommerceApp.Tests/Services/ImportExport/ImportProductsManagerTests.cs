using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportProductsManagerTests : InMemoryDatabaseTest
    {
        private readonly IImportProductsValidationService _importProductsValidationService;
        private readonly IImportProductsService _importProductsService;
        private readonly ImportProductsManager _importExportManager;

        public ImportProductsManagerTests()
        {
            _importProductsValidationService = A.Fake<IImportProductsValidationService>();
            _importProductsService = A.Fake<IImportProductsService>();

            _importExportManager = new ImportProductsManager(_importProductsValidationService, _importProductsService);
        }

        [Fact]
        public void ImportExportManager_ImportProductsFromExcel_ShouldNotBeNull()
        {
            var result = _importExportManager.ImportProductsFromExcel(GetDefaultStream(), true);

            result.Should().NotBeNull();
        }

        [Fact]
        public void ImportExportManager_ImportProductsFromExcel_ShouldReturnDictionary()
        {
            var result = _importExportManager.ImportProductsFromExcel(GetDefaultStream(), true);

            result.Should().HaveCount(0);
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}