using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website.ActionResults;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Admin.Controllers
{
    public class ListingControllerTests : MrCMSTest
    {
        private readonly IAmazonListingSyncManager _amazonListingSyncManager;
        private readonly IAmazonListingService _amazonListingService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly IPrepareForSyncAmazonListingService _prepareForSyncAmazonListingService;
        private readonly ListingController _listingController;

        public ListingControllerTests()
        {
            _amazonListingSyncManager = A.Fake<IAmazonListingSyncManager>();
            _amazonListingService = A.Fake<IAmazonListingService>();
            _amazonAppSettings = A.Fake<AmazonAppSettings>();
            _prepareForSyncAmazonListingService = A.Fake<IPrepareForSyncAmazonListingService>();
            _listingController = new ListingController(_amazonListingSyncManager, _amazonListingService,
            _amazonAppSettings, _prepareForSyncAmazonListingService);
        }

        [Fact]
        public void ListingController_Details_ReturnsViewResult()
        {
            var result = _listingController.Details(new AmazonListing());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_Details_ShouldRedirectToIndexIfNoListingProvided()
        {
            var result = _listingController.Details(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_ChooseProductVariant_ReturnsViewResult()
        {
            var result = _listingController.ChooseProductVariant(new AmazonListingGroup());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_ChooseProductVariant_ShouldCallGetAmazonDashboardModel()
        {
            var model = new AmazonListingGroup();

            var result = _listingController.ChooseProductVariant(model);

            A.CallTo(() => _amazonListingService.GetAmazonListingModel(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_AddOne_ShouldRedirectToIndexIfSkuNotProvided()
        {
            var result = _listingController.AddOne(null, 1);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_AddOne_ShouldRedirectToDetailsIfListingAlreadyAdded()
        {
            var result = _listingController.AddOne("T1", 1);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Details");
        }

        [Fact]
        public void ListingController_AddOne_ShouldCallGetGetByProductVariantSku()
        {
            var result = _listingController.AddOne("T1", 1);

            A.CallTo(() => _amazonListingService.GetByProductVariantSku("T1")).MustHaveHappened();
        }

        [Fact]
        public void ListingController_AddOne_ShouldReturnViewResultIfListingDoesntExist()
        {
            A.CallTo(() => _amazonListingService.GetByProductVariantSku("T1")).Returns(null);

            var result = _listingController.AddOne("T1", 1);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_AddOne_ShouldCallInitAmazonListingFromProductVariant()
        {
            A.CallTo(() => _amazonListingService.GetByProductVariantSku("T1")).Returns(null);

            var result = _listingController.AddOne("T1", 1);

            A.CallTo(() => _prepareForSyncAmazonListingService.InitAmazonListingFromProductVariant(null, "T1", 1)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_AddOnePOST_ShouldRedirectToIndexIfListingIsNull()
        {
            var result = _listingController.AddOne_POST(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_AddOnePOST_ShouldCallSave()
        {
            var listing = new AmazonListing();

            var result = _listingController.AddOne_POST(listing);

            A.CallTo(() => _amazonListingService.Save(listing)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_AddManyPOST_ShouldRedirectToSyncOneIfValuesProvided()
        {
            var listing = new AmazonListing();

            var result = _listingController.AddOne_POST(listing);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("SyncOne");
        }

        [Fact]
        public void ListingController_AddMany_ReturnsViewResult()
        {
            var result = _listingController.AddMany(new AmazonListingGroup());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_AddManyPOST_ShouldRedirectToIndexIfListingIsNull()
        {
            var result = _listingController.AddMany_POST(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_AddManyPOST_ShouldCallInitAmazonListingsFromProductVariants()
        {
            var model = new AmazonListingModel()
            {
                ChosenProductVariants = "P1",
                AmazonListingGroup = new AmazonListingGroup()
            };

            var result = _listingController.AddMany_POST(model);

            A.CallTo(() => _prepareForSyncAmazonListingService.InitAmazonListingsFromProductVariants(model.AmazonListingGroup, model.ChosenProductVariants)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_AddManyPOST_ShouldRedirectToSyncManyIfValuesProvided()
        {
            var model = new AmazonListingModel()
            {
                ChosenProductVariants = "P1",
                AmazonListingGroup = new AmazonListingGroup()
            };

            var result = _listingController.AddMany_POST(model);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("SyncMany");
        }

        [Fact]
        public void ListingController_SyncOneGET_ShouldRedirectToIndexIfListingIsNull()
        {
            var result = _listingController.SyncOne_GET(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_SyncOneGET_ShouldReturnViewResult()
        {
            var result = _listingController.SyncOne_GET(new AmazonListing());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_SyncOneGET_ShouldCallUpdateAmazonListing()
        {
            var listing = new AmazonListing();

            var result = _listingController.SyncOne_GET(listing);

            A.CallTo(() => _prepareForSyncAmazonListingService.UpdateAmazonListing(listing)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_SyncOneGET_ShouldCallGetAmazonSyncModel()
        {
            var model = new AmazonListing();

            var result = _listingController.SyncOne_GET(model);

            A.CallTo(() => _amazonListingSyncManager.GetAmazonSyncModel(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_SyncOne_ShouldReturnJsonResult()
        {
            var result = _listingController.SyncOne(new AmazonSyncModel());

            result.Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void ListingController_SyncOne_ShouldReturnFalseIfSyncModelIsNull()
        {
            var result = _listingController.SyncOne(null);

            result.As<JsonResult>().Data.Should().Be(false);
        }

        [Fact]
        public void ListingController_SyncOne_ShouldReturnTrueIfSyncModelIsNotNull()
        {
            var result = _listingController.SyncOne(new AmazonSyncModel());

            result.As<JsonResult>().Data.Should().Be(true);
        }

        [Fact]
        public void ListingController_SyncOne_ShouldCallSyncAmazonListing()
        {
            var model = new AmazonSyncModel();

            var result = _listingController.SyncOne(model);

            A.CallTo(() => _amazonListingSyncManager.SyncAmazonListing(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_SyncManyGET_ShouldRedirectToIndexIfListingIsNull()
        {
            var result = _listingController.SyncMany_GET(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_SyncManyGET_ShouldReturnViewResult()
        {
            var result = _listingController.SyncMany_GET(new AmazonListingGroup());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_SyncManyGET_ShouldCallUpdateAmazonListing()
        {
            var model = new AmazonListingGroup();

            var result = _listingController.SyncMany_GET(model);

            A.CallTo(() => _prepareForSyncAmazonListingService.UpdateAmazonListings(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_SyncManyGET_ShouldCallGetAmazonSyncModel()
        {
            var model = new AmazonListingGroup();

            var result = _listingController.SyncMany_GET(model);

            A.CallTo(() => _amazonListingSyncManager.GetAmazonSyncModel(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_SyncMany_ShouldReturnJsonResult()
        {
            var result = _listingController.SyncMany(new AmazonSyncModel());

            result.Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void ListingController_SyncMany_ShouldReturnFalseIfSyncModelIsNull()
        {
            var result = _listingController.SyncMany(null);

            result.As<JsonResult>().Data.Should().Be(false);
        }

        [Fact]
        public void ListingController_SyncMany_ShouldReturnTrueIfSyncModelIsNotNull()
        {
            var result = _listingController.SyncMany(new AmazonSyncModel());

            result.As<JsonResult>().Data.Should().Be(true);
        }

        [Fact]
        public void ListingController_SyncMany_ShouldCallSyncAmazonListings()
        {
            var model = new AmazonSyncModel();

            var result = _listingController.SyncMany(model);

            A.CallTo(() => _amazonListingSyncManager.SyncAmazonListings(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_CloseOne_ShouldRedirectToIndexIfListingIsNull()
        {
            var result = _listingController.CloseOne(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_CloseOne_ShouldReturnViewResult()
        {
            var result = _listingController.CloseOne(new AmazonListing());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_CloseOne_ShouldCallUpdateAmazonListing()
        {
            var listing = new AmazonListing();

            var result = _listingController.CloseOne(listing);

            A.CallTo(() => _prepareForSyncAmazonListingService.UpdateAmazonListing(listing)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_CloseOne_ShouldCallGetAmazonCloseModel()
        {
            var model = new AmazonListing();

            var result = _listingController.CloseOne(model);

            A.CallTo(() => _amazonListingSyncManager.GetAmazonSyncModel(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_CloseOnePOST_ShouldReturnJsonResult()
        {
            var result = _listingController.CloseOne_POST(new AmazonSyncModel());

            result.Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void ListingController_CloseOnePOST_ShouldReturnFalseIfCloseModelIsNull()
        {
            var result = _listingController.CloseOne_POST(null);

            result.As<JsonResult>().Data.Should().Be(false);
        }

        [Fact]
        public void ListingController_CloseOnePOST_ShouldReturnTrueIfCloseModelIsNotNull()
        {
            var result = _listingController.CloseOne_POST(new AmazonSyncModel());

            result.As<JsonResult>().Data.Should().Be(true);
        }

        [Fact]
        public void ListingController_CloseOnePOST_ShouldCallCloseAmazonListing()
        {
            var model = new AmazonSyncModel();

            var result = _listingController.CloseOne_POST(model);

            A.CallTo(() => _amazonListingSyncManager.CloseAmazonListing(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_CloseMany_ShouldRedirectToIndexIfListingIsNull()
        {
            var result = _listingController.CloseMany(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_CloseMany_ShouldReturnViewResult()
        {
            var result = _listingController.CloseMany(new AmazonListingGroup());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_CloseMany_ShouldCallUpdateAmazonListing()
        {
            var model = new AmazonListingGroup();

            var result = _listingController.CloseMany(model);

            A.CallTo(() => _prepareForSyncAmazonListingService.UpdateAmazonListings(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_CloseMany_ShouldCallGetAmazonSyncModel()
        {
            var model = new AmazonListingGroup();

            var result = _listingController.CloseMany(model);

            A.CallTo(() => _amazonListingSyncManager.GetAmazonSyncModel(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_CloseManyPOST_ShouldReturnJsonResult()
        {
            var result = _listingController.CloseMany_POST(new AmazonSyncModel());

            result.Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void ListingController_CloseManyPOST_ShouldReturnFalseIfSyncModelIsNull()
        {
            var result = _listingController.CloseMany_POST(null);

            result.As<JsonResult>().Data.Should().Be(false);
        }

        [Fact]
        public void ListingController_CloseManyPOST_ShouldReturnTrueIfSyncModelIsNotNull()
        {
            var result = _listingController.CloseMany_POST(new AmazonSyncModel());

            result.As<JsonResult>().Data.Should().Be(true);
        }

        [Fact]
        public void ListingController_CloseManyPOST_ShouldCallSyncAmazonListings()
        {
            var model = new AmazonSyncModel();

            var result = _listingController.CloseMany_POST(model);

            A.CallTo(() => _amazonListingSyncManager.CloseAmazonListings(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingController_Delete_ReturnsViewResult()
        {
            var result = _listingController.Delete(new AmazonListing());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingController_Delete_ShouldRedirectToIndexIfNoListingProvided()
        {
            var result = _listingController.Delete(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingController_DeletePOST_ReturnsRedirectToEdit()
        {
            var model = new AmazonListing()
            {
                AmazonListingGroup = new AmazonListingGroup() { Id = 1 }
            };

            var result = _listingController.Delete_POST(model);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
        }

        [Fact]
        public void ListingController_DeletePOST_ShouldCallDelete()
        {
            var model = new AmazonListing()
                {
                    AmazonListingGroup = new AmazonListingGroup() { Id = 1 }
                };

            var result = _listingController.Delete_POST(model);

            A.CallTo(() => _amazonListingService.Delete(model)).MustHaveHappened();
        }
    }
}