using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class CountryControllerTests
    {
        private CountryController _countryController;
        private ICountryService _countryService;

        public CountryControllerTests()
        {
            _countryService = A.Fake<ICountryService>();
            _countryController = new CountryController(_countryService);
        }

        [Fact]
        public void CountryController_Index_ReturnsAViewResult()
        {
            var index = _countryController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CountryController_Index_CallsCountryServiceGetAllCountries()
        {
            _countryController.Index();

            A.CallTo(() => _countryService.GetAllCountries()).MustHaveHappened();
        }

        [Fact]
        public void CountryController_Index_ReturnsTheResultOfGetAllCountriesAsTheModel()
        {
            var countries = new List<Country>();
            A.CallTo(() => _countryService.GetAllCountries()).Returns(countries);

            var index = _countryController.Index();

            index.Model.Should().Be(countries);
        }

        [Fact]
        public void CountryController_Add_ReturnsAPartialViewResult()
        {
            var add = _countryController.Add();

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void CountryController_Add_ShouldCallCountyServiceGetCountriesToAdd()
        {
            var add = _countryController.Add();

            A.CallTo(() => _countryService.GetCountriesToAdd()).MustHaveHappened();
        }

        [Fact]
        public void CountryController_Add_ReturnsCountriesToAddAsModel()
        {
            var selectListItems = new List<SelectListItem>();
            A.CallTo(() => _countryService.GetCountriesToAdd()).Returns(selectListItems);

            var add = _countryController.Add();

            add.Model.Should().Be(selectListItems);
        }

        [Fact]
        public void CountryController_AddPost_CallsAddCountryWithPassedValue()
        {
            _countryController.Add_POST("GB");

            A.CallTo(() => _countryService.AddCountry("GB")).MustHaveHappened();
        }

        [Fact]
        public void CountryController_AddPost_ReturnsRedirectToIndex()
        {
            var result = _countryController.Add_POST("GB");

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void CountryController_Edit_ReturnsPartialViewResult()
        {
            var result = _countryController.Edit(new Country());

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void CountryController_Edit_ReturnsPassedCountryAsModel()
        {
            var country = new Country();

            var result = _countryController.Edit(country);

            result.Model.Should().Be(country);
        }

        [Fact]
        public void CountryController_EditPost_ShouldCallSaveCountry()
        {
            var country = new Country();

            _countryController.Edit_POST(country);

            A.CallTo(() => _countryService.Save(country)).MustHaveHappened();
        }

        [Fact]
        public void CountryController_EditPost_RedirectsToIndex()
        {
            var country = new Country();

            var result = _countryController.Edit_POST(country);

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void CountryController_Delete_ReturnsPartialView()
        {
            var delete = _countryController.Delete(new Country());

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void CountryController_Delete_ShouldUsePassedCountryAsModel()
        {
            var country = new Country();

            var delete = _countryController.Delete(country);
            
            delete.Model.Should().Be(country);
        }

        [Fact]
        public void CountryController_DeletePOST_ShouldCallCountryServiceDeleteOnPassedCountry()
        {
            var country = new Country();

            _countryController.Delete_POST(country);

            A.CallTo(() => _countryService.Delete(country)).MustHaveHappened();
        }

        [Fact]
        public void CountryController_DeletePOST_ShouldRedirectToIndex()
        {
            var country = new Country();

            var result = _countryController.Delete_POST(country);

            result.RouteValues["action"].Should().Be("Index");
        }
    }
}