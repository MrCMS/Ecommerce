using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class TaxRateControllerTests
    {
        private readonly ITaxRateManager _taxRateManager;
        private readonly TaxRateController _taxRateController;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly TaxSettings _taxSettings;

        public TaxRateControllerTests()
        {
            _taxRateManager = A.Fake<ITaxRateManager>();
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _taxSettings = new TaxSettings();
            _taxRateController = new TaxRateController(_taxRateManager, _configurationProvider, _taxSettings);
        }

        [Fact]
        public void TaxRateController_Index_ReturnsViewResult()
        {
            var index = _taxRateController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void TaxRateController_Index_CallsITaxRateManagerGetAll()
        {
            var index = _taxRateController.Index();

            A.CallTo(() => _taxRateManager.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_Index_ReturnsTheResultOfGetAll()
        {
            var taxRates = new List<TaxRate>();
            A.CallTo(() => _taxRateManager.GetAll()).Returns(taxRates);

            var index = _taxRateController.Index();

            index.Model.ShouldBeEquivalentTo(taxRates);
        }

        [Fact]
        public void TaxRateController_Add_ReturnsPartialViewResult()
        {
            var add = _taxRateController.Add();

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Add_ReturnsATaxRate()
        {
            var add = _taxRateController.Add();

            add.Model.Should().BeOfType<TaxRate>();
        }

        [Fact]
        public void TaxRateController_AddPOST_CallsITaxRateManagerAddWithPassedTaxRate()
        {
            var taxRate = new TaxRate();

            var add = _taxRateController.Add_POST(taxRate);

            A.CallTo(() => _taxRateManager.Add(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_AddPOST_RedirectsToEditForTheIdOfTheSavedTaxRate()
        {
            var taxRate = new TaxRate { Id = 1 };

            var add = _taxRateController.Add_POST(taxRate);

            add.RouteValues["action"].Should().Be("Edit");
            add.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void TaxRateController_Edit_ReturnsAViewResult()
        {
            var edit = _taxRateController.Edit(new TaxRate());

            edit.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void TaxRateController_Edit_ShouldReturnPassedTaxRate()
        {
            var taxRate = new TaxRate();

            var edit = _taxRateController.Edit(taxRate);

            edit.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_EditPOST_ShouldCallITaxManagerUpdate()
        {
            var taxRate = new TaxRate();

            var editPost = _taxRateController.Edit_POST(taxRate);

            A.CallTo(() => _taxRateManager.Update(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_EditPOST_RedirectsToIndex()
        {
            var taxRate = new TaxRate { Id = 1 };

            var add = _taxRateController.Edit_POST(taxRate);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void TaxRateController_Delete_ReturnsAPartialView()
        {
            var delete = _taxRateController.Delete(new TaxRate());

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void TaxRateController_Delete_ShouldReturnPassedTaxRateAsModel()
        {
            var taxRate = new TaxRate();

            var delete = _taxRateController.Delete(taxRate);

            delete.Model.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateController_DeletePOST_ShouldCallDeleteForThePassedTaxRate()
        {
            var taxRate = new TaxRate();

            var delete = _taxRateController.Delete_POST(taxRate);

            A.CallTo(() => _taxRateManager.Delete(taxRate)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_DeletePOST_ShouldRedirectToIndex()
        {
            var taxRate = new TaxRate();

            var delete = _taxRateController.Delete_POST(taxRate);

            delete.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void TaxRateController_Settings_ShouldCallSaveSettingsOnTheConfigurationProvider()
        {
            var taxSettings = new TaxSettings();

            _taxRateController.Settings(taxSettings);

            A.CallTo(() => _configurationProvider.SaveSettings(taxSettings)).MustHaveHappened();
        }

        [Fact]
        public void TaxRateController_Settings_ShouldReturnARedirectToIndex()
        {
            var taxSettings = new TaxSettings();

            var result = _taxRateController.Settings(taxSettings);

            result.RouteValues["action"].Should().Be("Index");
        }
    }
}