using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ShippingCalculationControllerTests
    {
        private readonly ShippingCalculationController _shippingCalculationController;
        private readonly IShippingCalculationManager _shippingCalculationManager;
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly ICountryService _countryService;

        public ShippingCalculationControllerTests()
        {
            _shippingCalculationManager = A.Fake<IShippingCalculationManager>();
            _shippingMethodManager = A.Fake<IShippingMethodManager>();
            _countryService = A.Fake<ICountryService>();
            _shippingCalculationController = new ShippingCalculationController(_shippingCalculationManager, _shippingMethodManager, _countryService);
        }

        [Fact]
        public void ShippingCalculationController_Index_ReturnsViewResult()
        {
            ViewResult index = _shippingCalculationController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ShippingCalculationController_Index_CallsICountryServiceGetAllCountries()
        {
            ViewResult index = _shippingCalculationController.Index();

            A.CallTo(() => _countryService.GetAllCountries()).MustHaveHappened();
        }

        [Fact]
        public void ShippingCalculationController_Index_ReturnsTheResultOfGetAll()
        {
            var countries = new List<Country>();
            A.CallTo(() => _countryService.GetAllCountries()).Returns(countries);

            var index = _shippingCalculationController.Index();

            index.Model.Should().Be(countries);
        }

        [Fact]
        public void ShippingCalculationController_Add_ReturnsPartialViewResult()
        {
            ActionResult add = _shippingCalculationController.Add(new Country());

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingCalculationController_Add_ReturnsAShippingCalculation()
        {
            ActionResult add = _shippingCalculationController.Add(new Country());

            add.As<PartialViewResult>().Model.Should().BeOfType<ShippingCalculation>();
        }

        [Fact]
        public void ShippingCalculationController_AddPOST_CallsIShippingCalculationManagerAddWithPassedShippingCalculationIfItHasCountry()
        {
            var shippingCalculation = new ShippingCalculation {ShippingMethod = new ShippingMethod()};

            var add = _shippingCalculationController.Add_POST(shippingCalculation);

            A.CallTo(() => _shippingCalculationManager.Add(shippingCalculation)).MustHaveHappened();
        }

        [Fact]
        public void ShippingCalculationController_AddPOST_DoesNotCallIShippingCalculationManagerAddWithPassedShippingCalculationIfItHasNoCountryOrRegionSet()
        {
            var shippingCalculation = new ShippingCalculation();

            var add = _shippingCalculationController.Add_POST(shippingCalculation);

            A.CallTo(() => _shippingCalculationManager.Add(shippingCalculation)).MustNotHaveHappened();
        }

        [Fact]
        public void ShippingCalculationController_Edit_ReturnsAPartialViewResult()
        {
            var shippingCalculation = new ShippingCalculation();

            PartialViewResult edit = _shippingCalculationController.Edit(shippingCalculation);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingCalculationController_EditPOST_ShouldCallITaxManagerUpdateForAShippingCalculationWithACountry()
        {
            var shippingCalculation = new ShippingCalculation { ShippingMethod = new ShippingMethod() };

            var editPost = _shippingCalculationController.Edit_POST(shippingCalculation);

            A.CallTo(() => _shippingCalculationManager.Update(shippingCalculation)).MustHaveHappened();
        }

        [Fact]
        public void ShippingCalculationController_EditPOST_ShouldNotCallIShippingCalculationManagerUpdateForAShippingCalculationWithNoCountry()
        {
            var shippingCalculation = new ShippingCalculation();

            var editPost = _shippingCalculationController.Edit_POST(shippingCalculation);

            A.CallTo(() => _shippingCalculationManager.Update(shippingCalculation)).MustNotHaveHappened();
        }

        [Fact]
        public void ShippingCalculationController_Delete_ReturnsAPartialView()
        {
            var shippingCalculation = new ShippingCalculation();

            PartialViewResult delete = _shippingCalculationController.Delete(shippingCalculation);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingCalculationController_DeletePOST_ShouldCallDeleteForThePassedShippingCalculation()
        {
            var shippingCalculation = new ShippingCalculation();

            RedirectToRouteResult delete = _shippingCalculationController.Delete_POST(shippingCalculation);

            A.CallTo(() => _shippingCalculationManager.Delete(shippingCalculation)).MustHaveHappened();
        }

        [Fact]
        public void ShippingCalculationController_DeletePOST_ShouldRedirectToIndex()
        {
            var shippingCalculation = new ShippingCalculation();

            RedirectToRouteResult delete = _shippingCalculationController.Delete_POST(shippingCalculation);

            delete.RouteValues["action"].Should().Be("Index");
        }
    }
}