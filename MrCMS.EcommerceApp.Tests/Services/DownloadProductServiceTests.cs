using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class DownloadServiceTests : InMemoryDatabaseTest
    {
        private readonly DownloadProductVariantService _downloadProductService;
        private readonly IOrderService _orderService;
        private readonly IProductVariantService _productService;
        private IEnumerable<IDownloadProductValidationRule> _rules;
        private IEnumerable<IDownloadProductBasicValidationRule> _basicRules;

        public DownloadServiceTests()
        {
            var mockingKernel = new MockingKernel();
            _rules = Enumerable.Range(1, 10).Select(i => A.Fake<IDownloadProductValidationRule>()).ToList();
            _rules.ForEach(rule => mockingKernel.Bind<IDownloadProductValidationRule>().ToMethod(context => rule));
            _basicRules = Enumerable.Range(1, 10).Select(i => A.Fake<IDownloadProductBasicValidationRule>()).ToList();
            _basicRules.ForEach(rule => mockingKernel.Bind<IDownloadProductBasicValidationRule>().ToMethod(context => rule));
            mockingKernel.Bind<IFileService>()
                         .ToMethod(context => A.Fake<IFileService>())
                         .InSingletonScope();
            mockingKernel.Bind<MediaSettings>()
                        .ToMethod(context => A.Fake<MediaSettings>())
                        .InSingletonScope();
            MrCMSApplication.OverrideKernel(mockingKernel);
            _orderService = A.Fake<IOrderService>();
            _productService = A.Fake<IProductVariantService>();
            _downloadProductService = new DownloadProductVariantService(_orderService, _productService);
        }

        [Fact]
        public void DownloadService_Download_ShouldReturnFilePathResult()
        {
            var productVariant = new ProductVariant() { NumberOfDownloads = 0, DownloadFileUrl = "dlurl", DemoFileUrl = "demourl" };
            var order = new Order();

            var downloadFile = new MediaFile() { FileName = "dl", ContentType = "text/plain", FileUrl = "dlurl" };
            var demoFile = new MediaFile() { FileName = "demo", ContentType = "text/plain", FileUrl = "demourl" };

            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DownloadFileUrl)).Returns(downloadFile);
            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DemoFileUrl)).Returns(demoFile);

            var result = _downloadProductService.Download(productVariant, order,string.Empty);

            result.Should().BeOfType<FilePathResult>();
        }

        [Fact]
        public void DownloadService_Download_ShouldReturnFilePathResultForDemoFileIfTypeIsSpecified()
        {
            var productVariant = new ProductVariant() { NumberOfDownloads = 0, DownloadFileUrl = "dlurl", DemoFileUrl = "demourl" };
            var order = new Order();

            var downloadFile = new MediaFile() { FileName = "dl", ContentType = "text/plain", FileUrl = "dlurl" };
            var demoFile = new MediaFile() { FileName = "demo", ContentType = "text/plain", FileUrl = "demourl" };

            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DownloadFileUrl)).Returns(downloadFile);
            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DemoFileUrl)).Returns(demoFile);

            var result = _downloadProductService.Download(productVariant, order, "demo");

            result.As<FilePathResult>().FileName.Should().Be(demoFile.FileUrl);
        }

        [Fact]
        public void DownloadService_Download_ShouldCallUpdate()
        {
            var productVariant = new ProductVariant() { NumberOfDownloads = 0, DownloadFileUrl = "dlurl", DemoFileUrl = "demourl" };
            var order = new Order();


            var downloadFile = new MediaFile() { FileName = "dl", ContentType = "text/plain", FileUrl = "dlurl" };
            var demoFile = new MediaFile() { FileName = "demo", ContentType = "text/plain", FileUrl = "demourl" };

            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DownloadFileUrl)).Returns(downloadFile);
            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(productVariant.DemoFileUrl)).Returns(demoFile);

            var result = _downloadProductService.Download(productVariant, order, string.Empty);

            productVariant.NumberOfDownloads++;
            A.CallTo(() => _productService.Update(productVariant)).MustHaveHappened();
        }
    }
}