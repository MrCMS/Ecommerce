using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ExportProductsManagerTests : InMemoryDatabaseTest
    {
        private readonly ExportProductsManager _exportProductsManager;
        private readonly IProductVariantService _productVariantService;
        private IGetStockRemainingQuantity _getStockRemainingQuantity;

        public ExportProductsManagerTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();

            _getStockRemainingQuantity = A.Fake<IGetStockRemainingQuantity>();
            _exportProductsManager = new ExportProductsManager(_productVariantService, _getStockRemainingQuantity);
        }

        [Fact]
        public void ImportExportManager_ExportProductsToExcel_ShouldReturnByteArray()
        {
            byte[] result = _exportProductsManager.ExportProductsToExcel();

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