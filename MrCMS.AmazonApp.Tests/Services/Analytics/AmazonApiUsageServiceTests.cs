using System.Linq;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Website;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Analytics
{
    public class AmazonApiUsageServiceTests : InMemoryDatabaseTest
    {
        private AmazonApiUsageService _amazonApiUsageService;

        public AmazonApiUsageServiceTests()
        {
            _amazonApiUsageService = new AmazonApiUsageService(Session);
        }

        [Fact]
        public void AmazonApiUsageService_Save_ShouldUpdateEntry()
        {
            var item = new AmazonApiUsage() { NoOfCalls = 1};
            Session.Transact(session => session.Save(item));

            item.NoOfCalls = 2;
            _amazonApiUsageService.Save(item);
            Session.Evict(item);

            Session.QueryOver<AmazonApiUsage>().SingleOrDefault().NoOfCalls.Should().Be(2);
        }

        [Fact]
        public void AmazonApiUsageService_GetForToday_ReturnsPersistedEntryWhichMatchesCriteria()
        {
            var items = Enumerable.Range(0, 9).Select(i => new AmazonApiUsage()
                {
                    ApiSection = AmazonApiSection.Feeds, 
                    ApiOperation = "SubmitFeed", 
                    NoOfCalls = i+1, 
                    Day = CurrentRequestData.Now.Date.AddDays(i),
                    Site = CurrentRequestData.CurrentSite
                }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonApiUsageService.GetForToday(AmazonApiSection.Feeds, "SubmitFeed");

            results.Should().Be(items.First());
        }
    }
}