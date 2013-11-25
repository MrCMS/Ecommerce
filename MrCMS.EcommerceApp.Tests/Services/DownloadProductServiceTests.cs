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
using MrCMS.Web.Apps.Ecommerce.Pages;
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
        private readonly DownloadProductService _downloadProductService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
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
            _productService = A.Fake<IProductService>();
            _downloadProductService = new DownloadProductService(_orderService, _productService);
        }

        [Fact]
        public void DownloadService_Download_ShouldReturnFilePathResult()
        {
            var product = new Product(){NumberOfDownloads = 0, DownloadFileUrl = "dlurl",DemoFileUrl = "demourl"};
            var productVariant = new ProductVariant(){Product = product};
            var order = new Order();

            var downloadFile = new MediaFile() { FileName = "dl", ContentType = "text/plain", FileUrl = "dlurl" };
            var demoFile = new MediaFile() { FileName = "demo", ContentType = "text/plain", FileUrl = "demourl" };

            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(product.DownloadFileUrl)).Returns(downloadFile);
            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(product.DemoFileUrl)).Returns(demoFile);

            var result = _downloadProductService.Download(productVariant, order,string.Empty);

            result.Should().BeOfType<FilePathResult>();
        }

        [Fact]
        public void DownloadService_Download_ShouldCallUpdate()
        {
            var product = new Product() { NumberOfDownloads = 0, DownloadFileUrl = "dlurl", DemoFileUrl = "demourl" };
            var productVariant = new ProductVariant() { Product = product };
            var order = new Order();

            var downloadFile = new MediaFile() { FileName = "dl", ContentType = "text/plain", FileUrl = "dlurl" };
            var demoFile = new MediaFile() { FileName = "demo", ContentType = "text/plain", FileUrl = "demourl" };

            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(product.DownloadFileUrl)).Returns(downloadFile);
            A.CallTo(() => MrCMSApplication.Get<IFileService>().GetFileByUrl(product.DemoFileUrl)).Returns(demoFile);

            var result = _downloadProductService.Download(productVariant, order, string.Empty);

            productVariant.Product.NumberOfDownloads++;
            A.CallTo(() => _productService.Update(productVariant.Product)).MustHaveHappened();
        }
    }
}