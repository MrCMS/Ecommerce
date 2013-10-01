using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Settings;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Admin.Controllers
{
    public class ListingGroupControllerTests : InMemoryDatabaseTest
    {
        private readonly IAmazonListingGroupService _amazonListingGroupService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly ListingGroupController _listingGroupController;

        public ListingGroupControllerTests()
        {
            _amazonListingGroupService = A.Fake<IAmazonListingGroupService>();
            _amazonAppSettings = A.Fake<AmazonAppSettings>();
            _listingGroupController = new ListingGroupController(_amazonListingGroupService,_amazonAppSettings);
        }

        [Fact]
        public void ListingGroupController_Index_ReturnsViewResult()
        {
            var result = _listingGroupController.Index(null);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingGroupController_Index_ShouldCallSearch()
        {
            var result = _listingGroupController.Index(null,1);

            A.CallTo(() => _amazonListingGroupService.Search(null,1,10)).MustHaveHappened();
        }

        [Fact]
        public void ListingGroupController_ListingGroups_ReturnsPartialViewResult()
        {
            var result = _listingGroupController.ListingGroups(null);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ListingGroupController_ListingGroups_ShouldCallSearch()
        {
            var result = _listingGroupController.Index(null, 1);

            A.CallTo(() => _amazonListingGroupService.Search(null, 1, 10)).MustHaveHappened();
        }

        [Fact]
        public void ListingGroupController_Listings_ReturnsPartialViewResult()
        {
            var result = _listingGroupController.Listings(new AmazonListingGroup());

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ListingGroupController_Add_ReturnsPartialViewResult()
        {
            var result = _listingGroupController.Listings(new AmazonListingGroup());

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ListingGroupController_AddPOST_ShouldRedirectToEdit()
        {
            var result = _listingGroupController.Add_POST(new AmazonListingGroup(){Id=1});

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
        }

        [Fact]
        public void ListingGroupController_Edit_ReturnsViewResult()
        {
            var result = _listingGroupController.Edit(new AmazonListingGroup());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ListingGroupController_Edit_ShouldRedirectToIndexIfNoListingGroupProvided()
        {
            var result = _listingGroupController.Edit(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingGroupController_EditPOST_ReturnsRedirectToEdit()
        {
            var model = new AmazonListingGroup() {Id = 1};

            var result = _listingGroupController.Edit_POST(model);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
        }

        [Fact]
        public void ListingGroupController_EditPOST_ShouldCallSave()
        {
            var model = new AmazonListingGroup() { Id = 1 };

            var result = _listingGroupController.Edit_POST(model);

            A.CallTo(() => _amazonListingGroupService.Save(model)).MustHaveHappened();
        }

        [Fact]
        public void ListingGroupController_DeletePOST_ReturnsRedirectToEdit()
        {
            var model = new AmazonListingGroup() { Id = 1 };

            var result = _listingGroupController.Delete_POST(model);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ListingGroupController_DeletePOST_ShouldCallDelete()
        {
            var model = new AmazonListingGroup() { Id = 1 };

            var result = _listingGroupController.Delete_POST(model);

            A.CallTo(() => _amazonListingGroupService.Delete(model)).MustHaveHappened();
        }
    }
}