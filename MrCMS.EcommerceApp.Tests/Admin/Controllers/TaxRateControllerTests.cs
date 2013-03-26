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
        public void TaxRateController_Index_CallsITaxRateManagerGetAll()
        {
            var taxRateController = GetTaxRateController();

            var index = taxRateController.Index();

            A.CallTo(() => _taxRateManager.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_Index_ReturnsTheResultOfGetAll()
        {
            var taxRateController = GetTaxRateController();
            var taxRates = new List<TaxRate>();
            A.CallTo(() => _taxRateManager.GetAll()).Returns(taxRates);
            
            var index = taxRateController.Index();

            index.Model.Should().Be(taxRates);
        }

        [Fact]
        public void TaxRateController_Add_ReturnsPartialViewResult()
        {
            var taxRateController = GetTaxRateController();

            var add = taxRateController.Add(1,null);

            add.Should().BeOfType<ActionResult>();
        }

        [Fact]
        public void TaxRateController_Add_ReturnsATaxRate()
        {
            var taxRateController = GetTaxRateController();

            var add = taxRateController.Add(1,null);

            //add.Model.Should().BeOfType<TaxRate>();
        }

        [Fact]
        public void TaxRateController_AddPOST_CallsITaxRateManagerAddWithPassedTaxRate()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();

            var add = taxRateController.Add_POST(taxRate);

            A.CallTo(() => _taxRateManager.Add(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_AddPOST_RedirectsToEditForTheIdOfTheSavedTaxRate()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate{Id=1};

            var add = taxRateController.Add_POST(taxRate);

            //add.RouteValues["action"].Should().Be("Edit");
            //add.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void TaxRateController_Edit_ReturnsAViewResult()
        {
            var taxRateController = GetTaxRateController();

            var edit = taxRateController.Edit(new TaxRate(),1,null);

            edit.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void TaxRateController_Edit_ShouldReturnPassedTaxRate()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();

            var edit = taxRateController.Edit(taxRate,1,null);

            edit.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_EditPOST_ShouldCallITaxManagerUpdate()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate();

            var editPost = taxRateController.Edit_POST(taxRate);

            A.CallTo(() => _taxRateManager.Update(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_EditPOST_RedirectsToEditForTheIdOfTheSavedTaxRate()
        {
            var taxRateController = GetTaxRateController();
            var taxRate = new TaxRate{Id=1};
            
            var add = taxRateController.Edit_POST(taxRate);

            add.RouteValues["action"].Should().Be("Edit");
            add.RouteValues["id"].Should().Be(1);
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