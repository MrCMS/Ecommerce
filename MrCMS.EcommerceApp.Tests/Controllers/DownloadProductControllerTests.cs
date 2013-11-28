using System.Collections.Generic;
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
        public void IfServiceGetDownloadReturnsAValueReturnsThat()
        {
            var order = new Order();
            var orderLine = new OrderLine();
            var filePathResult = new FilePathResult("test", "test");
            A.CallTo(() => _downloadProductService.GetDownload(order, orderLine)).Returns(filePathResult);

            var actionResult = _downloadOrderedFileController.Download(order, orderLine);

            actionResult.Should().Be(filePathResult);
        }
        
        [Fact]
        public void IfServiceGetDownloadReturnsNullReturnAnEmptyResult()
        {
            var order = new Order();
            var orderLine = new OrderLine();
            A.CallTo(() => _downloadProductService.GetDownload(order, orderLine)).Returns(null);

            var actionResult = _downloadOrderedFileController.Download(order, orderLine);

            actionResult.Should().BeOfType<EmptyResult>();
        }
    }
}