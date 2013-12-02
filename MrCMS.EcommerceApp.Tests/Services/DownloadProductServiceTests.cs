using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules;
using MrCMS.Website;
using NHibernate;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class DownloadOrderedFileService_GetDownloadTests
    {
        private readonly DownloadOrderedFileService _downloadProductService;
        private readonly List<IDownloadOrderedFileValidationRule> _rules = new List<IDownloadOrderedFileValidationRule>();
        private readonly IFileSystem _fileSystem;
        private readonly ISession _session;

        public DownloadOrderedFileService_GetDownloadTests()
        {
            _fileSystem = A.Fake<IFileSystem>();
            _session = A.Fake<ISession>();
            _downloadProductService = new DownloadOrderedFileService(_session, _rules, _fileSystem);
        }

        [Fact]
        public void IfOrderIsNullResponseStreamShouldNotBeWrittenTo()
        {
            HttpResponseBase response = new OutOfContextResponse();

            _downloadProductService.WriteDownloadToResponse(response, null, new OrderLine());

            A.CallTo(() => _fileSystem.WriteToStream(A<string>._, A<Stream>._)).MustNotHaveHappened();
        }

        [Fact]
        public void IfOrderLineIsNullStreamShouldNotBeWrittenTo()
        {
            HttpResponseBase response = new OutOfContextResponse();

            _downloadProductService.WriteDownloadToResponse(response, new Order(), null);

            A.CallTo(() => _fileSystem.WriteToStream(A<string>._, A<Stream>._)).MustNotHaveHappened();
        }

        [Fact]
        public void IfAnyRuleErrorsStreamShouldNotBeWrittenTo()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().Build();
            HttpResponseBase response = new OutOfContextResponse();
            SetRulesStatus(order, orderLine, false);

            _downloadProductService.WriteDownloadToResponse(response, order, orderLine);

            A.CallTo(() => _fileSystem.WriteToStream(A<string>._, A<Stream>._)).MustNotHaveHappened();
        }

        [Fact]
        public void IfFileDoesNotExistStreamShouldNotBeWrittenTo()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithFileUrl("test-file-url").Build();
            HttpResponseBase response = new OutOfContextResponse();
            A.CallTo(() => _fileSystem.Exists("test-file-url")).Returns(false);

            _downloadProductService.WriteDownloadToResponse(response, order, orderLine);

            A.CallTo(() => _fileSystem.WriteToStream(A<string>._, A<Stream>._)).MustNotHaveHappened();
        }

        [Fact]
        public void IfAllChecksPassUnbufferTheResponse()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithFileUrl("test-file-url").Build();
            HttpResponseBase response = new OutOfContextResponse { Buffer = true };
            A.CallTo(() => _fileSystem.Exists("test-file-url")).Returns(true);

            _downloadProductService.WriteDownloadToResponse(response, order, orderLine);

            response.Buffer.Should().BeFalse();
        }

        [Fact]
        public void IfAllChecksPassSetContentDispositionHeaderWithFileName()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithFileUrl("test-file-url").WithFileName("test-file-name").Build();
            HttpResponseBase response = new OutOfContextResponse();
            A.CallTo(() => _fileSystem.Exists("test-file-url")).Returns(true);

            _downloadProductService.WriteDownloadToResponse(response, order, orderLine);

            response.Headers.AllKeys.Should().Contain("Content-Disposition");
            response.Headers["Content-Disposition"].Should()
                                                   .Be("attachment; filename=test-file-name");
        }

        [Fact]
        public void IfAllChecksPassCallFileSystemWriteToOutputStream()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithFileUrl("test-file-url").WithFileName("test-file-name").Build();
            HttpResponseBase response = new OutOfContextResponse();
            A.CallTo(() => _fileSystem.Exists("test-file-url")).Returns(true);

            _downloadProductService.WriteDownloadToResponse(response, order, orderLine);

            A.CallTo(() => _fileSystem.WriteToStream("test-file-url", response.OutputStream)).MustHaveHappened();
        }

        [Fact]
        public void IfAllChecksPassIncreaseTheDownloadCount()
        {
            var order = new Order();
            var orderLine = new OrderLineBuilder().WithFileUrl("test-file-url").WithFileName("test-file-name").WithNumberOfDownloads(1).Build();
            HttpResponseBase response = new OutOfContextResponse();
            A.CallTo(() => _fileSystem.Exists("test-file-url")).Returns(true);

            _downloadProductService.WriteDownloadToResponse(response, order, orderLine);

            orderLine.NumberOfDownloads.Should().Be(2);
            A.CallTo(() => _session.Update(orderLine)).MustHaveHappened();
        }

        private void SetRulesStatus(Order order, OrderLine orderLine, bool passing)
        {
            var downloadOrderedFileValidationRule = A.Fake<IDownloadOrderedFileValidationRule>();
            var errors = passing ? new List<string>() : new List<string> { "an error" };
            A.CallTo(() => downloadOrderedFileValidationRule.GetErrors(order, orderLine)).Returns(errors);
            _rules.Add(downloadOrderedFileValidationRule);
        }
    }
}