using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Logs
{
    public class AmazonLogServiceTests : InMemoryDatabaseTest
    {
        private AmazonLogService _amazonLogService;

        public AmazonLogServiceTests()
        {
            _amazonLogService = new AmazonLogService(Session);
        }

        [Fact]
        public void AmazonLogService_Add_ShouldSaveEntry()
        {
            _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Insert, null, null, null, null, null, null, null);

            Session.QueryOver<AmazonLog>().RowCount().Should().Be(1);
        }

        [Fact]
        public void AmazonLogService_GetEntriesPaged_ShouldReturnPersistedEntries()
        {
            for (var i = 0; i < 10; i++)
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Insert, null, null, null, null, null, null,null);

            Session.QueryOver<AmazonLog>().RowCount().Should().Be(10);
        }
    }
}