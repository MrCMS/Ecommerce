using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Admin.Controllers
{
    public class LogsControllerTests : InMemoryDatabaseTest
    {
        private readonly IAmazonLogService _amazonLogService;
        private readonly LogsController _logsController;
        private readonly SiteSettings _siteSettings;

        public LogsControllerTests()
        {
            _amazonLogService = A.Fake<IAmazonLogService>();
            _siteSettings = new SiteSettings() { DefaultPageSize = 10 };
            _logsController = new LogsController(_amazonLogService, _siteSettings);
        }

        [Fact]
        public void LogsController_Index_ReturnsViewResult()
        {
            var result = _logsController.Index(null,null);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LogsController_Index_ShouldCallSearch()
        {
            var result = _logsController.Index(null,null);

            A.CallTo(() => _amazonLogService.GetEntriesPaged(1,null,null,10)).MustHaveHappened();
        }

        [Fact]
        public void LogsController_IndexPOST_ShouldRedirectToIndex()
        {
            var result = _logsController.Index_POST(null, null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void LogsController_Details_ReturnsViewResult()
        {
            var result = _logsController.Details(new AmazonLog());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LogsController_DeleteAllLogs_ReturnsPartialViewResult()
        {
            var result = _logsController.DeleteAllLogs();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void LogsController_DeleteAllLogsPOST_ShouldCallDeleteAllLogs()
        {
            var result = _logsController.DeleteAllLogs_POST();

            A.CallTo(() => _amazonLogService.DeleteAllLogs()).MustHaveHappened();
        }

        [Fact]
        public void LogsController_DeleteAllLogsPOST_ShouldCallAddLog()
        {
            var result = _logsController.DeleteAllLogs_POST();

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.Logs, AmazonLogStatus.Delete, null,null,null, null, null, null, null, null, string.Empty)).MustHaveHappened();
        }

        [Fact]
        public void LogsController_DeleteAllLogsPOST_ShouldRedirectToIndex()
        {
            var result = _logsController.DeleteAllLogs_POST();

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }
    }
}