using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class RegionControllerTests
    {
        private RegionController _regionController;
        private IRegionService _regionService;

        public RegionControllerTests()
        {
            _regionService = A.Fake<IRegionService>();
            _regionController = new RegionController(_regionService);
        }

        [Fact]
        public void RegionController_Add_ShouldReturnPartialViewResult()
        {
            var country = new Country();
            
            var result = _regionController.Add(country);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void RegionController_Add_ShouldReturnANewRegionAsAModel()
        {
            var country = new Country();
            
            var result = _regionController.Add(country);

            result.Model.Should().BeOfType<Region>();
        }

        [Fact]
        public void RegionController_Add_ModelShouldHavePassedCountrySet()
        {
            var country = new Country();
            
            var result = _regionController.Add(country);

            result.Model.As<Region>().Country.Should().Be(country);
        }

        [Fact]
        public void RegionController_AddPost_ShouldCallAddRegion()
        {
            var region = new Region();
            
            var post = _regionController.Add_POST(region);

            A.CallTo(() => _regionService.Add(region)).MustHaveHappened();
        }

        [Fact]
        public void RegionController_AddPost_ShouldRedirectToCountryIndex()
        {
            var region = new Region();

            var post = _regionController.Add_POST(region);

            post.RouteValues["action"].Should().Be("Index");
            post.RouteValues["controller"].Should().Be("Country");
        }

        [Fact]
        public void RegionController_Edit_ReturnsAPartialView()
        {
            var region = new Region();

            var edit = _regionController.Edit(region);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void RegionController_Edit_ReturnsThePassedObject()
        {
            var region = new Region();

            var edit = _regionController.Edit(region);

            edit.Model.Should().Be(region);
        }

        [Fact]
        public void RegionController_EditPOST_ShouldCallSaveRegion()
        {
            var region = new Region();
            
           _regionController.Edit_POST(region);

            A.CallTo(() => _regionService.Update(region)).MustHaveHappened();
        }

        [Fact]
        public void RegionController_EditPOST_ShouldReturnRedirectToCountryIndex()
        {
            var region = new Region();

            var result = _regionController.Edit_POST(region);

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Country");
        }

        [Fact]
        public void RegionController_Delete_ShouldReturnAPartialViewResult()
        {
            var region = new Region();

            var delete = _regionController.Delete(region);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void RegionController_Delete_ShouldReturnPassedRegionAsTheModel()
        {
            var region = new Region();

            var delete = _regionController.Delete(region);

            delete.Model.Should().Be(region);
        }

        [Fact]
        public void RegionController_DeletePOST_ShouldCallRegionServiceDelete()
        {
            var region = new Region();

            _regionController.Delete_POST(region);

            A.CallTo(() => _regionService.Delete(region)).MustHaveHappened();
        }

        [Fact]
        public void RegionController_DeletePOST_ShouldRedirectToCountryIndex()
        {
            var region = new Region();

            var result = _regionController.Delete_POST(region);

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Country");
        }
    }
}