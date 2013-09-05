using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Ninject.MockingKernel;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportExportManagerTests
    {
        private readonly IImportProductsValidationService _importProductsValidationService;
        private readonly IImportProductsService _importProductsService;
        private readonly IProductVariantService _productVariantService;
        private readonly ImportExportManager _importExportManager;
        private readonly IOrderShippingService _orderShippingService;
        private readonly ISession _session;
        private MockingKernel _mockingKernel;

        public ImportExportManagerTests()
        {
            _mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(_mockingKernel);
            CurrentRequestData.CurrentUser = new User() { FirstName = "Test", LastName = "User" };
            CurrentRequestData.CurrentSite = new Site() { Name = "Test", BaseUrl = "www.example.com"};
            CurrentRequestData.SiteSettings = new SiteSettings() { TimeZone = "GMT Standard Time", UICulture = "en-GB" };

            _importProductsValidationService = A.Fake<IImportProductsValidationService>();
            _importProductsService = A.Fake<IImportProductsService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _orderShippingService = A.Fake<IOrderShippingService>();
            _session = A.Fake<ISession>();

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