using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Listings.Sync
{
    public class AmazonListingSyncManagerTests : InMemoryDatabaseTest
    {
        private ISyncAmazonListingService _syncAmazonListingService;
        private IAmazonListingService _amazonListingService;
        private IAmazonLogService _amazonLogService;
        private ICloseAmazonListingService _closeAmazonListingService;
        private IAmazonListingGroupService _amazonListingGroupService;
        private AmazonListingSyncManager _amazonListingSyncManager;

        public AmazonListingSyncManagerTests()
        {
            _syncAmazonListingService = A.Fake<ISyncAmazonListingService>();
            _amazonListingService = A.Fake<IAmazonListingService>();
            _amazonLogService = A.Fake<IAmazonLogService>();
            _closeAmazonListingService = A.Fake<ICloseAmazonListingService>();
            _amazonListingGroupService = A.Fake<IAmazonListingGroupService>();
            _amazonListingSyncManager = new AmazonListingSyncManager(_syncAmazonListingService,_amazonListingService,
                _amazonLogService,_closeAmazonListingService,_amazonListingGroupService);
        }

        [Fact]
        public void AmazonListingSyncManager_GetAmazonSyncModel_ShouldReturnAmazonSyncModelType()
        {
            var model = new AmazonListingGroup();

            var results = _amazonListingSyncManager.GetAmazonSyncModel(model);

            results.Should().BeOfType<AmazonSyncModel>();
        }

        [Fact]
        public void AmazonListingSyncManager_GetAmazonSyncModel_ShouldReturnAmazonSyncModelTypeSecondMethod()
        {
            var model = new AmazonListing();

            var results = _amazonListingSyncManager.GetAmazonSyncModel(model);

            results.Should().BeOfType<AmazonSyncModel>();
        }
    }
}