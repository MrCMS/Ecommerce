using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ExportProductsManagerTests : InMemoryDatabaseTest
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ExportProductsManager _exportProductsManager;

        public ExportProductsManagerTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();

            _exportProductsManager = new ExportProductsManager(_productVariantService);
        }

        [Fact]
        public void ImportExportManager_ExportProductsToExcel_ShouldReturnByteArray()
        {
            var result = _exportProductsManager.ExportProductsToExcel();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ImportExportManager_ExportProductsToExcel_ShouldCallGetAllOfProductVariantService()
        {
            _exportProductsManager.ExportProductsToExcel();

            A.CallTo(() => _productVariantService.GetAll()).MustHaveHappened();
        }
    }
}