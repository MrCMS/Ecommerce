using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportExportManagerTests : InMemoryDatabaseTest
    {
        private IProductService _productService;
        private IProductVariantService _productVariantService;
        private IDocumentService _documentService;
        private IProductOptionManager _productOptionManager;
        private IBrandService _brandService;
        private ITaxRateManager _taxRateManager;
        private IFileService _fileService;
        private IIndexService _indexService;

        [Fact]
        public void ImportExportManage_ExportProductsToExcel_ShouldReturnByteArray()
        {
            var importExportManager = GetImportExportManager();

            var result = importExportManager.ExportProductsToExcel();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ImportExportManage_ExportProductsToExcel_ShouldCallGetAllOfProductVariantService()
        {
            var importExportManager = GetImportExportManager();

            var result = importExportManager.ExportProductsToExcel();

            A.CallTo(() => _productVariantService.GetAll()).MustHaveHappened();
        }

        ImportExportManager GetImportExportManager()
        {
            _productService = A.Fake<IProductService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _documentService = A.Fake<IDocumentService>();
            _productOptionManager = A.Fake<IProductOptionManager>();
            _brandService = A.Fake<IBrandService>();
            _taxRateManager = A.Fake<ITaxRateManager>();
            _fileService = A.Fake<IFileService>();
            _indexService = A.Fake<IIndexService>();

            return new ImportExportManager(_productService, _productVariantService,
            _documentService, _productOptionManager, _brandService, _taxRateManager,
            _fileService, _indexService);
        }
    }
}