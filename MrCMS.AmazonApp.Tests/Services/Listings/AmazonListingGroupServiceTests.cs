using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Listings
{
    public class AmazonListingGroupServiceTests : InMemoryDatabaseTest
    {
        private IAmazonLogService _amazonLogService;
        private AmazonListingGroupService _amazonListingGroupService;

        public AmazonListingGroupServiceTests()
        {
            _amazonLogService = A.Fake<IAmazonLogService>();
            _amazonListingGroupService = new AmazonListingGroupService(Session, _amazonLogService);
        }

        [Fact]
        public void AmazonListingGroupService_Get_ShouldReturnPersistedEntryFromSession()
        {
            var item = new AmazonListingGroup();
            Session.Transact(session => session.Save(item));

            var results=_amazonListingGroupService.Get(1);

            results.As<AmazonListingGroup>().Id.Should().Be(1);
        }

        [Fact]
        public void AmazonListingGroupService_Save_ShouldUpdateInSession()
        {
            var item = new AmazonListingGroup();
            Session.Transact(session => session.Save(item));
            item.Name = "updated";

            _amazonListingGroupService.Save(item);
            Session.Evict(item);

            Session.Get<AmazonListingGroup>(1).Name.Should().Be("updated");
        }

        [Fact]
        public void AmazonListingGroupService_Save_ShouldCallAddLog()
        {
            var item = new AmazonListingGroup();

            _amazonListingGroupService.Save(item);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.ListingGroups, AmazonLogStatus.Insert,
                                  null, null, null, null, null, null, item, string.Empty, string.Empty)).MustHaveHappened();
        }

        [Fact]
        public void AmazonListingGroupService_Delete_ShouldRemoveItemFromTheSession()
        {
            var item = new AmazonListingGroup();
            Session.Transact(session => session.Save(item));

            _amazonListingGroupService.Delete(item);

            Session.QueryOver<AmazonListingGroup>().RowCount().Should().Be(0);
        }


        [Fact]
        public void AmazonListingGroupService_Delete_ShouldCallAddLog()
        {
            var item = new AmazonListingGroup();

            _amazonListingGroupService.Delete(item);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.ListingGroups, AmazonLogStatus.Delete, null, null, null, null, null, null, item, string.Empty, string.Empty)).MustHaveHappened();
        }


        [Fact]
        public void AmazonListingGroupService_Search_ShouldReturnPersistedEntries()
        {
            var items = Enumerable.Range(0,10).Select(i => new AmazonListingGroup()
                {
                    Name=i.ToString()
                }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonListingGroupService.Search();

            results.Should().HaveCount(10);
        }
    }
}