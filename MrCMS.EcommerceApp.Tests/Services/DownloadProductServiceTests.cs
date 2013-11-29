using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class DownloadOrderedFileService_GetDownloadTests : InMemoryDatabaseTest
    {
        private readonly DownloadOrderedFileService _downloadProductService;
        private readonly List<IDownloadOrderedFileValidationRule> _rules = new List<IDownloadOrderedFileValidationRule>();

        public DownloadOrderedFileService_GetDownloadTests()
        {
            _downloadProductService = new DownloadOrderedFileService(Session, _rules);
        }

        [Fact]
        public void IfOrderIsNullReturnsNull()
        {
            var result = _downloadProductService.GetDownload(null, new OrderLine());

            result.Should().BeNull();
        }

        [Fact]
        public void IfOrderLineIsNullReturnsNull()
        {
            var result = _downloadProductService.GetDownload(new Order(), null);

            result.Should().BeNull();
        }

        [Fact]
        public void IfAnyRuleErrorsReturnNull()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().Build();
            var downloadOrderedFileValidationRule = A.Fake<IDownloadOrderedFileValidationRule>();
            A.CallTo(() => downloadOrderedFileValidationRule.GetErrors(order, orderLine)).Returns(new List<string> { "an error" });
            _rules.Add(downloadOrderedFileValidationRule);

            var result = _downloadProductService.GetDownload(order, orderLine);

            result.Should().BeNull();
        }

        [Fact]
        public void IfAllRulesPassReturnASuccessfulFilePathResult()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().Build();
            Session.Transact(session => session.Save(orderLine));
            var downloadOrderedFileValidationRule = A.Fake<IDownloadOrderedFileValidationRule>();
            A.CallTo(() => downloadOrderedFileValidationRule.GetErrors(order, orderLine)).Returns(new List<string>());
            _rules.Add(downloadOrderedFileValidationRule);

            var downloadValidationResult = _downloadProductService.GetDownload(order, orderLine);

            downloadValidationResult.Should().NotBeNull();
            downloadValidationResult.Should().BeOfType<FilePathResult>();
        }

        [Fact]
        public void IfValidFileDownloadNameShouldBeOrderLineFileName()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithFileName("test-file-name").Build();
            Session.Transact(session => session.Save(orderLine));

            var downloadValidationResult = _downloadProductService.GetDownload(order, orderLine);

            downloadValidationResult.FileDownloadName.Should().Be("test-file-name");
        }

        [Fact]
        public void IfValidFileContentTypeShouldBeOrderLineContentType()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithContentType("test-content-type").Build();
            Session.Transact(session => session.Save(orderLine));

            var downloadValidationResult = _downloadProductService.GetDownload(order, orderLine);

            downloadValidationResult.ContentType.Should().Be("test-content-type");
        }

        [Fact]
        public void IfValidFileFileNameShouldBeOrderLineFileUrl()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithFileUrl("test-file-url").Build();
            Session.Transact(session => session.Save(orderLine));

            var downloadValidationResult = _downloadProductService.GetDownload(order, orderLine);

            downloadValidationResult.FileName.Should().Be("test-file-url");
        }
    }

    public class OrderLineBuilder
    {
        private string _downloadFileUrl;
        private string _downloadContentType;
        private string _downloadFileName;

        public OrderLineBuilder()
        {
            _downloadFileUrl = "test-url";
            _downloadContentType = "content-type";
            _downloadFileName = "file-name";
        }

        public OrderLineBuilder WithFileUrl(string fileUrl)
        {
            _downloadFileUrl = fileUrl;
            return this;
        }

        public OrderLineBuilder WithContentType(string contentType)
        {
            _downloadContentType = contentType;
            return this;
        }

        public OrderLineBuilder WithFileName(string fileName)
        {
            _downloadFileName = fileName;
            return this;
        }

        public OrderLine Build()
        {
            return new OrderLine
                       {
                           DownloadFileUrl = _downloadFileUrl,
                           DownloadFileContentType = _downloadContentType,
                           DownloadFileName = _downloadFileName
                       };
        }
    }
}