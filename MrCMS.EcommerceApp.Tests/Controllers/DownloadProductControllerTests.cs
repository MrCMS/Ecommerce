using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Controllers
{
    public class DownloadOrderedFileController_DownloadTests
    {
        private readonly IDownloadOrderedFileService _downloadProductService;
        private readonly DownloadOrderedFileController _downloadOrderedFileController;

        public DownloadOrderedFileController_DownloadTests()
        {
            _downloadProductService = A.Fake<IDownloadOrderedFileService>();
            _downloadOrderedFileController = new DownloadOrderedFileController(_downloadProductService);
        }

        [Fact]
        public void ACallToWriteDownloadToResponseShouldHappen()
        {
            var order = new Order();
            var orderLine = new OrderLine();

            _downloadOrderedFileController.Download(order, orderLine);

            A.CallTo(() => _downloadProductService.WriteDownloadToResponse(_downloadOrderedFileController.Response, order, orderLine)).MustHaveHappened();
        }

        [Fact]
        public void ReturnsTheResultOfServiceCall()
        {
            var order = new Order();
            var orderLine = new OrderLine();
            var stubFileStreamResult = new StubFileStreamResult();
            A.CallTo(
                () =>
                _downloadProductService.WriteDownloadToResponse(_downloadOrderedFileController.Response, order,
                                                                orderLine)).Returns(stubFileStreamResult);

            var actionResult = _downloadOrderedFileController.Download(order, orderLine);

            actionResult.Should().Be(stubFileStreamResult);
        }
    }

    public class StubFileStreamResult : FileStreamResult
    {
        public StubFileStreamResult()
            : base(new MemoryStream(), "stub")
        {
        }

        protected override void WriteFile(HttpResponseBase response)
        {

        }
    }
}