using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class TaxRateControllerTests
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ICountryService _countryService;
        private readonly IRegionService _regionService;
        private readonly TaxRateController _taxRateController;
        private readonly ITaxRateManager _taxRateManager;
        private readonly TaxSettings _taxSettings;

        public TaxRateControllerTests()
        {
            _taxRateManager = A.Fake<ITaxRateManager>();
            _countryService = A.Fake<ICountryService>();
            _regionService = A.Fake<IRegionService>();
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _taxSettings = new TaxSettings();
            _taxRateController = new TaxRateController(_taxRateManager, _countryService, _regionService,
                                                       _configurationProvider,
                                                       _taxSettings);
        }

        [Fact]
        public void TaxRateController_Index_ReturnsViewResult()
        {
            ViewResult index = _taxRateController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void TaxRateController_Index_CallsICountryServiceGetAllCountries()
        {
            ViewResult index = _taxRateController.Index();

            A.CallTo(() => _countryService.GetAllCountries()).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_Index_ReturnsTheResultOfGetAllCountries()
        {
            var countries = new List<Country>();
            A.CallTo(() => _countryService.GetAllCountries()).Returns(countries);

            ViewResult index = _taxRateController.Index();

            index.Model.Should().Be(countries);
        }

        [Fact]
        public void TaxRateController_Add_ReturnsPartialViewResult()
        {
            ActionResult add = _taxRateController.Add(1, null);

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Add_ReturnsATaxRate()
        {
            ActionResult add = _taxRateController.Add(1, null);

            add.As<PartialViewResult>().Model.Should().BeOfType<TaxRate>();
        }

        [Fact]
        public void TaxRateController_AddPOST_CallsITaxRateManagerAddWithPassedTaxRateIfItHasCountry()
        {
            var taxRate = new TaxRate {Country = new Country()};

            RedirectToRouteResult add = _taxRateController.Add_POST(taxRate);

            A.CallTo(() => _taxRateManager.Add(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_AddPOST_DoesNotCallITaxRateManagerAddWithPassedTaxRateIfItHasNoCountryOrRegionSet()
        {
            var taxRate = new TaxRate {};

            RedirectToRouteResult add = _taxRateController.Add_POST(taxRate);

            A.CallTo(() => _taxRateManager.Add(taxRate)).MustNotHaveHappened();
        }

        [Fact]
        public void TaxRateController_AddPOST_RedirectsToTheIndex()
        {
            var taxRate = new TaxRate {Id = 1};

            RedirectToRouteResult add = _taxRateController.Add_POST(taxRate);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void TaxRateController_Edit_ReturnsAPartialViewResult()
        {
            PartialViewResult edit = _taxRateController.Edit(1, null);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Edit_IfCountryIdIsSetReturnsTheResultOfGetByCountryIdAsTheModel()
        {
            var taxRate = new TaxRate();
            A.CallTo(() => _taxRateManager.GetByCountryId(1)).Returns(taxRate);

            PartialViewResult edit = _taxRateController.Edit(1, null);

            edit.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_Edit_IfRegionIdIsSetReturnsTheResultOfGetByRegionIdAsTheModel()
        {
            var taxRate = new TaxRate();
            A.CallTo(() => _taxRateManager.GetByRegionId(1)).Returns(taxRate);

            PartialViewResult edit = _taxRateController.Edit(null, 1);

            edit.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_EditPOST_ShouldCallITaxManagerUpdateForATaxRateWithACountry()
        {
            var taxRate = new TaxRate {Country = new Country()};

            RedirectToRouteResult editPost = _taxRateController.Edit_POST(taxRate);

            A.CallTo(() => _taxRateManager.Update(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_EditPOST_ShouldNotCallITaxManagerUpdateForATaxRateWithNoCountryOrRegion()
        {
            var taxRate = new TaxRate();

            RedirectToRouteResult editPost = _taxRateController.Edit_POST(taxRate);

            A.CallTo(() => _taxRateManager.Update(taxRate)).MustNotHaveHappened();
        }

        [Fact]
        public void TaxRateController_EditPOST_RedirectsToTheIndex()
        {
            var taxRate = new TaxRate {Id = 1};

            RedirectToRouteResult add = _taxRateController.Edit_POST(taxRate);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void TaxRateController_Delete_ReturnsAPartialView()
        {
            PartialViewResult delete = _taxRateController.Delete(null, null);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Delete_ShouldReturnTheResultOfGetByCountryIdIfItIsSet()
        {
            var taxRate = new TaxRate();
            A.CallTo(() => _taxRateManager.GetByCountryId(1)).Returns(taxRate);

            PartialViewResult delete = _taxRateController.Delete(1, null);

            delete.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_Delete_ShouldReturnTheResultOfGetByRegionIdIfItIsSet()
        {
            var taxRate = new TaxRate();
            A.CallTo(() => _taxRateManager.GetByRegionId(1)).Returns(taxRate);

            PartialViewResult delete = _taxRateController.Delete(null, 1);

            delete.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_DeletePOST_ShouldCallDeleteForThePassedTaxRate()
        {
            var taxRate = new TaxRate();

            RedirectToRouteResult delete = _taxRateController.Delete_POST(taxRate);

            A.CallTo(() => _taxRateManager.Delete(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_DeletePOST_ShouldRedirectToIndex()
        {
            var taxRate = new TaxRate();

            RedirectToRouteResult delete = _taxRateController.Delete_POST(taxRate);

            delete.RouteValues["action"].Should().Be("Index");
        }
    }
}