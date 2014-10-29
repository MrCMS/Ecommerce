using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using Xunit;
using System;

namespace MrCMS.AmazonApp.Tests.Admin.Controllers
{
    public class AppControllerTests : MrCMSTest
    {
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly AppController _appController;
        private readonly SiteSettings _siteSettings;

        public AppControllerTests()
        {
            _amazonLogService = A.Fake<IAmazonLogService>();
            _amazonAnalyticsService = A.Fake<IAmazonAnalyticsService>();
            _siteSettings=new SiteSettings(){DefaultPageSize = 10};
            _appController = new AppController(_amazonLogService, _amazonAnalyticsService, _siteSettings);
        }

        [Fact]
        public void AppController_Dashboard_ReturnsViewResult()
        {
            var result = _appController.Dashboard(DateTime.Now.AddDays(-7),DateTime.Now);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void AppController_Dashboard_ShouldCallGetAmazonDashboardModel()
        {
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;

            var result = _appController.Dashboard(from, to);

            A.CallTo(() => _amazonAnalyticsService.GetAmazonDashboardModel(from, to)).MustHaveHappened();
        }

        [Fact]
        public void AppController_DashboardPOST_ReturnsViewResult()
        {
            var result = _appController.Dashboard_POST(DateTime.Now.AddDays(-7), DateTime.Now);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void AppController_DashboardPOST_ShouldCallGetAmazonDashboardModel()
        {
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;

            var result = _appController.Dashboard_POST(from, to);

            A.CallTo(() => _amazonAnalyticsService.GetAmazonDashboardModel(from, to)).MustHaveHappened();
        }

        [Fact]
        public void AppController_DashboardLogs_ReturnsPartialViewResult()
        {
            var result = _appController.Logs(1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void AppController_DashboardLogs_ShouldCallGetEntriesPaged()
        {
            var result = _appController.Logs(1);

            A.CallTo(() => _amazonLogService.GetEntriesPaged(1,null,null,_siteSettings.DefaultPageSize)).MustHaveHappened();
        }

        [Fact]
        public void AppController_Revenue_ReturnsJsonResult()
        {
            var result = _appController.Revenue(DateTime.Now.AddDays(-7), DateTime.Now);

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void AppController_Revenue_ShouldCallGetAmazonDashboardModel()
        {
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;

            var result = _appController.Revenue(from, to);

            A.CallTo(() => _amazonAnalyticsService.GetRevenue(from, to)).MustHaveHappened();
        }

        [Fact]
        public void AppController_ProductsSold_ReturnsJsonResult()
        {
            var result = _appController.ProductsSold(DateTime.Now.AddDays(-7), DateTime.Now);

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void AppController_ProductsSold_ShouldCallGetAmazonDashboardModel()
        {
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;

            var result = _appController.ProductsSold(from, to);

            A.CallTo(() => _amazonAnalyticsService.GetProductsSold(from, to)).MustHaveHappened();
        }

        [Fact]
        public void AppController_ProgressBarStatus_ReturnsJsonResult()
        {
            var result = _appController.ProgressBarStatus(new AmazonSyncModel(){TaskId = Guid.NewGuid()});

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void AppController_ProgressBarMessages_ReturnsPartialViewResult()
        {
            var result = _appController.ProgressBarMessages(new AmazonSyncModel() { TaskId = Guid.NewGuid() });

            result.Should().BeOfType<PartialViewResult>();
        }
    }
}