using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportExportManagerTests : InMemoryDatabaseTest
    {
        private IImportProductsValidationService _importProductsValidationService;
        private IImportProductsService _importProductsService;
        private IProductVariantService _productVariantService;
        private IIndexService _indexService;

        [Fact]
        public void ImportExportManager_ExportProductsToExcel_ShouldReturnByteArray()
        {
            var importExportManager = GetImportExportManager();

            var result = importExportManager.ExportProductsToExcel();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ImportExportManager_ExportProductsToExcel_ShouldCallGetAllOfProductVariantService()
        {
            var importExportManager = GetImportExportManager();

            importExportManager.ExportProductsToExcel();

            A.CallTo(() => _productVariantService.GetAll()).MustHaveHappened();
        }

        ImportExportManager GetImportExportManager()
        {
            _importProductsValidationService = A.Fake<IImportProductsValidationService>();
            _importProductsService = A.Fake<IImportProductsService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _indexService = A.Fake<IIndexService>();

            return new ImportExportManager(_importProductsValidationService, _importProductsService, _productVariantService,
            _indexService);
        }
    }
}