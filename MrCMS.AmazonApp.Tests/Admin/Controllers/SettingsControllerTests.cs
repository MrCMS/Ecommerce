using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Admin.Controllers
{
    public class SettingsControllerTests : InMemoryDatabaseTest
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAmazonLogService _amazonLogService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly SettingsController _settingsController;
        private readonly AmazonSyncSettings _amazonSyncSettings;

        public SettingsControllerTests()
        {
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _amazonLogService = A.Fake<IAmazonLogService>();
            _amazonAppSettings = A.Fake<AmazonAppSettings>();
            _amazonSellerSettings = A.Fake<AmazonSellerSettings>();
             _amazonSyncSettings = A.Fake<AmazonSyncSettings>();
             _settingsController = new SettingsController(_configurationProvider, _amazonLogService, _amazonAppSettings, _amazonSellerSettings, _amazonSyncSettings);
        }

        [Fact]
        public void SettingsController_App_ReturnsViewResult()
        {
            var result = _settingsController.App();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void SettingsController_AppPOST_ShouldCallSaveSettings()
        {
            var model = new AmazonAppSettings();

            var result = _settingsController.App_POST(model);

            A.CallTo(() => _configurationProvider.SaveSettings(model)).MustHaveHappened();
        }

        [Fact]
        public void SettingsController_AppPOST_ShouldCallAddLog()
        {
            var model = new AmazonAppSettings();

            var result = _settingsController.App_POST(model);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.AppSettings, AmazonLogStatus.Update, 
                null, null, null, null, null, null,null, string.Empty,string.Empty)).MustHaveHappened();
        }

        [Fact]
        public void SettingsController_Seller_ReturnsViewResult()
        {
            var result = _settingsController.Seller();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void SettingsController_SellerPOST_ShouldCallSaveSettings()
        {
            var model = new AmazonSellerSettings();

            var result = _settingsController.Seller_POST(model);

            A.CallTo(() => _configurationProvider.SaveSettings(model)).MustHaveHappened();
        }

        [Fact]
        public void SettingsController_SellerPOST_ShouldCallAddLog()
        {
            var model = new AmazonSellerSettings();

            var result = _settingsController.Seller_POST(model);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.SellerSettings, AmazonLogStatus.Update,
                null, null, null, null, null, null, null, null, string.Empty)).MustHaveHappened();
        }
    }
}