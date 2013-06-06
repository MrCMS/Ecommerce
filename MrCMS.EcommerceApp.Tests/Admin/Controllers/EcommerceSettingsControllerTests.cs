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
    public class EcommerceSettingsControllerTests
    {
        private readonly EcommerceSettingsController _ecommerceSettingsController;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly EcommerceSettings _ecommerceSettings;

        public EcommerceSettingsControllerTests()
        {
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _ecommerceSettings = new EcommerceSettings();
            _ecommerceSettingsController = new EcommerceSettingsController(_configurationProvider, _ecommerceSettings);
        }

        [Fact]
        public void EcommerceSettingsController_Edit_ShouldCallSaveSettingsOnTheConfigurationProvider()
        {
            var settings = new EcommerceSettings();

            var edit=_ecommerceSettingsController.Edit();

            edit.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void EcommerceSettingsController_EditPOST_ShouldCallSaveSettingsOnTheConfigurationProvider()
        {
            var settings = new EcommerceSettings();

            _ecommerceSettingsController.Edit_POST(settings);

            A.CallTo(() => _configurationProvider.SaveSettings(settings)).MustHaveHappened();
        }
    }
}