using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class TaxRateControllerTests
    {
        private ITaxRateManager _taxRateManager;
        private ICountryService _countryService;
        private IRegionService _regionService;

        [Fact]
        public void TaxRateController_Index_ReturnsViewResult()
        {
            var taxRateController = GetTaxRateController();

            var index = taxRateController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void TaxRateController_Index_CallsICountryServiceGetAllCountries()
        {
            var taxRateController = GetTaxRateController();

            var index = taxRateController.Index();

            A.CallTo(() => _countryService.GetAllCountries()).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_Index_ReturnsTheResultOfGetAllCountries()
        {
            var taxRateController = GetTaxRateController();
            var countries = new List<Country>();
            A.CallTo(() => _countryService.GetAllCountries()).Returns(countries);
            
            var index = taxRateController.Index();

            index.Model.Should().Be(countries);
        }

        [Fact]
        public void TaxRateController_Add_ReturnsPartialViewResult()
        {
            var taxRateController = GetTaxRateController();

            var add = taxRateController.Add(1,null);

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Add_ReturnsATaxRate()
        {
            var taxRateController = GetTaxRateController();

            var add = taxRateController.Add(1,null);

            add.As<PartialViewResult>().Model.Should().BeOfType<TaxRate>();
        }

        [Fact]
        public void TaxRateController_AddPOST_CallsITaxRateManagerAddWithPassedTaxRateIfItHasCountry()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate{Country = new Country()};

            var add = taxRateController.Add_POST(taxRate);

            A.CallTo(() => _taxRateManager.Add(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_AddPOST_DoesNotCallITaxRateManagerAddWithPassedTaxRateIfItHasNoCountryOrRegionSet()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate{};

            var add = taxRateController.Add_POST(taxRate);

            A.CallTo(() => _taxRateManager.Add(taxRate)).MustNotHaveHappened();
        }

        [Fact]
        public void TaxRateController_AddPOST_RedirectsToTheIndex()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate{Id=1};

            var add = taxRateController.Add_POST(taxRate);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void TaxRateController_Edit_ReturnsAPartialViewResult()
        {
            var taxRateController = GetTaxRateController();

            var edit = taxRateController.Edit(1,null);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Edit_IfCountryIdIsSetReturnsTheResultOfGetByCountryIdAsTheModel()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();
            A.CallTo(() => _taxRateManager.GetByCountryId(1)).Returns(taxRate);

            var edit = taxRateController.Edit(1, null);

            edit.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_Edit_IfRegionIdIsSetReturnsTheResultOfGetByRegionIdAsTheModel()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();
            A.CallTo(() => _taxRateManager.GetByRegionId(1)).Returns(taxRate);

            var edit = taxRateController.Edit(null, 1);

            edit.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_EditPOST_ShouldCallITaxManagerUpdateForATaxRateWithACountry()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate{Country = new Country()};

            var editPost = taxRateController.Edit_POST(taxRate);

            A.CallTo(() => _taxRateManager.Update(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_EditPOST_ShouldNotCallITaxManagerUpdateForATaxRateWithNoCountryOrRegion()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();

            var editPost = taxRateController.Edit_POST(taxRate);

            A.CallTo(() => _taxRateManager.Update(taxRate)).MustNotHaveHappened();
        }

        [Fact]
        public void TaxRateController_EditPOST_RedirectsToTheIndex()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate{Id=1};
            
            var add = taxRateController.Edit_POST(taxRate);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void TaxRateController_Delete_ReturnsAPartialView()
        {
            var taxRateController = GetTaxRateController();

            var delete = taxRateController.Delete(new TaxRate(), null, null);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Delete_ShouldReturnPassedTaxRateAsModel()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();

            var delete = taxRateController.Delete(taxRate, null, null);

            delete.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_DeletePOST_ShouldCallDeleteForThePassedTaxRate()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();
            
            var delete = taxRateController.Delete_POST(taxRate);

            A.CallTo(() => _taxRateManager.Delete(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_DeletePOST_ShouldRedirectToIndex()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();
            
            var delete = taxRateController.Delete_POST(taxRate);

            delete.RouteValues["action"].Should().Be("Index");
        }

        TaxRateController GetTaxRateController()
        {
            _taxRateManager = A.Fake<ITaxRateManager>();
            _countryService = A.Fake<ICountryService>();
            return new TaxRateController(_taxRateManager, _countryService, _regionService);
        }
    }
}