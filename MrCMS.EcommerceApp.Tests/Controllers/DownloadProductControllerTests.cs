using System;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class DownloadProductControllerTests
    {
        private readonly IDownloadProductVariantService _downloadProductService;
        private readonly DownloadProductController _downloadProductController;

        public DownloadProductControllerTests()
        {
            var mockingKernel = new MockingKernel();
            mockingKernel.Bind<IFileService>()
                         .ToMethod(context => A.Fake<IFileService>())
                         .InSingletonScope();
            mockingKernel.Bind<MediaSettings>()
                        .ToMethod(context => A.Fake<MediaSettings>())
                        .InSingletonScope();
            MrCMSApplication.OverrideKernel(mockingKernel);
            _downloadProductService = A.Fake<IDownloadProductVariantService>();
            _downloadProductController = new DownloadProductController(_downloadProductService);
        }

        [Fact]
        public void DownloadProductController_Download_ReturnsFilePathResultIfValidationIsOk()
        {
            var oguid = Guid.NewGuid().ToString();
            var productVariant = new ProductVariant() { NumberOfDownloads = 0, DownloadFileUrl = "dlurl", DemoFileUrl = "demourl" };
            Order order = null;

            var downloadFile = new MediaFile() { FileName = "dl", ContentType = "text/plain", FileUrl = "dlurl" };
            var demoFile = new MediaFile() { FileName = "demo", ContentType = "text/plain", FileUrl = "demourl" };

            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DownloadFileUrl)).Returns(downloadFile);
            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DemoFileUrl)).Returns(demoFile);

            A.CallTo(() => _downloadProductService.Validate(ref order, oguid, productVariant, string.Empty)).Returns(null);

            A.CallTo(() => _downloadProductService.Download(productVariant, order, string.Empty)).Returns(new FilePathResult(productVariant.DownloadFile.FileUrl,
                    productVariant.DownloadFile.ContentType) { FileDownloadName = productVariant.DownloadFile.FileName });

            var result = _downloadProductController.Download(oguid, productVariant, String.Empty);

            result.Should().BeOfType<FilePathResult>();
        }

        [Fact]
        public void DownloadProductController_Download_ShouldCallValidate()
        {
            var oguid = Guid.NewGuid().ToString();
            var productVariant = new ProductVariant();
            Order order = null;

            var result = _downloadProductController.Download(oguid, productVariant, String.Empty);

            A.CallTo(() => _downloadProductService.Validate(ref order, oguid, productVariant, string.Empty)).MustHaveHappened();
        }

        [Fact]
        public void DownloadProductController_Download_ShouldCallDownloadIfValidationIsOk()
        {
            var oguid = Guid.NewGuid().ToString();
            var productVariant = new ProductVariant();
            Order order = null;

            A.CallTo(() => _downloadProductService.Validate(ref order, oguid, productVariant, string.Empty)).Returns(null);

            var result = _downloadProductController.Download(oguid, productVariant, String.Empty);

            A.CallTo(() => _downloadProductService.Download(productVariant, order, string.Empty)).MustHaveHappened();
        }
    }
}