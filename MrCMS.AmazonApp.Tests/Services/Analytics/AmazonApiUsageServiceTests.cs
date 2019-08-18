using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Analytics
{
    public class AmazonApiUsageServiceTests : InMemoryDatabaseTest
    {
        private AmazonApiLogService _amazonApiUsageService;

        public AmazonApiUsageServiceTests()
        {
            _amazonApiUsageService = new AmazonApiLogService(Session);
        }

        [Fact]
        public void AmazonApiUsageService_Save_ShouldUpdateEntry()
        {
            var item = new AmazonApiLog() { ApiSection = AmazonApiSection.Feeds};
            Session.Transact(session => session.Save(item));

            item.ApiSection = AmazonApiSection.Orders;
            _amazonApiUsageService.Save(item);
            Session.Evict(item);

            Session.QueryOver<AmazonApiLog>().SingleOrDefault().ApiSection.Should().Be(AmazonApiSection.Orders);
        }
    }
}