using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportExportManagerTests : InMemoryDatabaseTest
    {
        private readonly IImportProductsValidationService _importProductsValidationService;
        private readonly IImportProductsService _importProductsService;
        private readonly IProductVariantService _productVariantService;
        private readonly ImportExportManager _importExportManager;
        private readonly IOrderShippingService _orderShippingService;

        public ImportExportManagerTests()
        {
            _importProductsValidationService = A.Fake<IImportProductsValidationService>();
            _importProductsService = A.Fake<IImportProductsService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _orderShippingService = A.Fake<IOrderShippingService>();

            _importExportManager = new ImportExportManager(_importProductsValidationService, _importProductsService, _productVariantService, _orderShippingService);
        }

        [Fact]
        public void ImportExportManager_ImportProductsFromExcel_ShouldNotBeNull()
        {
            var result = _importExportManager.ImportProductsFromExcel(GetDefaultStream());

            result.Should().NotBeNull();
        }

        [Fact]
        public void ImportExportManager_ImportProductsFromExcel_ShouldReturnDictionary()
        {
            var result = _importExportManager.ImportProductsFromExcel(GetDefaultStream());

            result.Should().HaveCount(0);
        }

        [Fact]
        public void ImportExportManager_ExportProductsToExcel_ShouldReturnByteArray()
        {
            var result = _importExportManager.ExportProductsToExcel();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ImportExportManager_ExportProductsToExcel_ShouldCallGetAllOfProductVariantService()
        {
            _importExportManager.ExportProductsToExcel();

            A.CallTo(() => _productVariantService.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void ImportExportManager_ExportProductsToGoogleBase_ShouldNotBeNull()
        {
            var result = _importExportManager.ExportProductsToGoogleBase();

            result.Should().NotBeNull();
        }

        [Fact]
        public void ImportExportManager_ExportProductsToGoogleBase_ShouldReturnByteArray()
        {
            var result = _importExportManager.ExportProductsToGoogleBase();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ImportExportManager_ExportProductsToGoogleBase_ShouldCallGetAllVariantsOfProductVariantService()
        {
            _importExportManager.ExportProductsToGoogleBase();

            A.CallTo(() => _productVariantService.GetAllVariants(string.Empty,0,1)).MustHaveHappened();
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}