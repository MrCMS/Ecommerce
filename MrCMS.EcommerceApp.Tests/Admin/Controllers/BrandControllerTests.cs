using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;
using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Paging;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class BrandControllerTests : InMemoryDatabaseTest
    {
        private readonly IBrandService _brandService;
        private readonly BrandController _brandController;

        public BrandControllerTests()
        {
            _brandService = A.Fake<IBrandService>();
            _brandController = new BrandController(_brandService);
        }

        [Fact]
        public void BrandController_Index_ReturnsViewResult()
        {
            var index = _brandController.Index(String.Empty);

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void BrandController_Index_CallsIBrandServiceGetPaged()
        {
            var index = _brandController.Index(String.Empty,1);

            A.CallTo(() => _brandService.GetPaged(1,String.Empty,10)).MustHaveHappened();
        }

        [Fact]
        public void BrandController_Index_ReturnsTheResultOfGetPaged()
        {
            var brands = new PagedList<Brand>(new List<Brand>(),1,10);
            A.CallTo(() => _brandService.GetPaged(1, String.Empty, 10)).Returns(brands);

            var index = _brandController.Index(String.Empty);

            index.Model.Should().Be(brands);
        }

        [Fact]
        public void BrandController_Add_ReturnsPartialViewResult()
        {
            var add = _brandController.Add();

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void BrandController_Add_ReturnsABrand()
        {
            var add = _brandController.Add();

            add.Model.Should().BeOfType<Brand>();
        }

        [Fact]
        public void BrandController_AddPOST_CallsIBrandServiceAddWithPassedBrand()
        {
            var brand = new Brand();

            var add = _brandController.Add_POST(brand);

            A.CallTo(() => _brandService.Add(brand)).MustHaveHappened();
        }

        [Fact]
        public void BrandController_AddPOST_RedirectsToEditForTheIdOfTheSavedBrand()
        {
            var brand = new Brand { Id = 1 };

            var add = _brandController.Add_POST(brand);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void BrandController_Edit_ReturnsAViewResult()
        {
            var edit = _brandController.Edit(new Brand());

            edit.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void BrandController_Edit_ShouldReturnPassedBrand()
        {
            var brand = new Brand();

            var edit = _brandController.Edit(brand);

            edit.Model.Should().Be(brand);
        }

        [Fact]
        public void BrandController_EditPOST_ShouldCallITaxServiceUpdate()
        {
            var brand = new Brand();

            var editPost = _brandController.Edit_POST(brand);

            A.CallTo(() => _brandService.Update(brand)).MustHaveHappened();
        }

        [Fact]
        public void BrandController_EditPOST_RedirectsToEditForTheIdOfTheSavedBrand()
        {
            var brand = new Brand { Id = 1 };

            var edit = _brandController.Edit_POST(brand);

            edit.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void BrandController_Delete_ReturnsAPartialView()
        {
            var delete = _brandController.Delete(new Brand());

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void BrandController_Delete_ShouldReturnPassedBrandAsModel()
        {
            var brand = new Brand();

            var delete = _brandController.Delete(brand);

            delete.Model.Should().Be(brand);
        }

        [Fact]
        public void BrandController_DeletePOST_ShouldCallDeleteForThePassedBrand()
        {
            var brand = new Brand();

            var delete = _brandController.Delete_POST(brand);

            A.CallTo(() => _brandService.Delete(brand)).MustHaveHappened();
        }

        [Fact]
        public void BrandController_DeletePOST_ShouldRedirectToIndex()
        {
            var brand = new Brand();

            var delete = _brandController.Delete_POST(brand);

            delete.RouteValues["action"].Should().Be("Index");
        }
    }
}