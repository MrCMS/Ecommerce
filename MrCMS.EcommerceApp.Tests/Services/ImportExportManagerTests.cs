using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using OfficeOpenXml;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportExportManagerTests : InMemoryDatabaseTest
    {
        private IImportProductsValidationService _importProductsValidationService;
        private IImportProductsService _importProductsService;
        private IProductVariantService _productVariantService;
        private IIndexService _indexService;
        private ImportExportManager _importExportManager;

        public ImportExportManagerTests()
        {
            _importProductsValidationService = A.Fake<IImportProductsValidationService>();
            _importProductsService = A.Fake<IImportProductsService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _indexService = A.Fake<IIndexService>();

            _importExportManager = new ImportExportManager(_importProductsValidationService, _importProductsService, _productVariantService,
                                                           _indexService);
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
        public void ImportExportManager_ImportProductsFromExcel_ShouldCallGetIndexesOfIndexService()
        {
            _importExportManager.ImportProductsFromExcel(GetDefaultStream());

            A.CallTo(() => _indexService.GetIndexes()).MustHaveHappened();
        }

        [Fact]
        public void ImportExportManager_ImportProductsFromExcel_ShouldCallImportProductsFromDTOsOfImportProductsService()
        {
            _importExportManager.ImportProductsFromExcel(GetDefaultStream());

            A.CallTo(() => _importProductsService.ImportProductsFromDTOs(new List<ProductImportDataTransferObject>())).MustHaveHappened();
        }

        [Fact]
        public void ImportExportManager_ImportProductsFromExcel_ShouldCallValidateBusinessLogicOfImportProductsValidationService()
        {
            var productsToImport = A.Fake<List<ProductImportDataTransferObject>>();

            _importExportManager.ImportProductsFromExcel(GetDefaultStream());

            A.CallTo(() => _importProductsValidationService.ValidateBusinessLogic(productsToImport)).MustHaveHappened();
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

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}