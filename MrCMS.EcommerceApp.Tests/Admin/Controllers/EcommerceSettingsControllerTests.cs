using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class EcommerceSettingsControllerTests
    {
        private readonly IEcommerceSettingsAdminService _ecommerceSettingsAdminService;
        private readonly EcommerceSettingsController _ecommerceSettingsController;

        public EcommerceSettingsControllerTests()
        {
            _ecommerceSettingsAdminService = A.Fake<IEcommerceSettingsAdminService>();
            _ecommerceSettingsController = new EcommerceSettingsController(_ecommerceSettingsAdminService);
        }

        [Fact]
        public void EcommerceSettingsController_Edit_ShouldCallSaveSettingsOnTheConfigurationProvider()
        {
            ActionResult edit = _ecommerceSettingsController.Edit();

            edit.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void EcommerceSettingsController_EditPOST_ShouldCallSaveSettingsOnTheService()
        {
            var settings = new EcommerceSettings();

            _ecommerceSettingsController.Edit_POST(settings);

            A.CallTo(() => _ecommerceSettingsAdminService.SaveSettings(settings)).MustHaveHappened();
        }
    }
}