using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Templating;
using MrCMS.Web.Apps.Ecommerce.Services.Templating;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class NotificationTemplateSettingsControllerTests
    {
        private readonly NotificationTemplateSettingsController _notificationTemplateSettingsController;
        private readonly IMessageTemplateSettingsManager _notificationTemplateSettingsManager;

        public NotificationTemplateSettingsControllerTests()
        {
            _notificationTemplateSettingsManager = A.Fake<IMessageTemplateSettingsManager>();
            _notificationTemplateSettingsController =
                new NotificationTemplateSettingsController(_notificationTemplateSettingsManager);
        }

        [Fact]
        public void NotificationTemplateSettingsController_Edit_CallsManagersGetMethod()
        {
            _notificationTemplateSettingsController.Edit();

            A.CallTo(() => _notificationTemplateSettingsManager.Get()).MustHaveHappened();
        }

        [Fact]
        public void NotificationTemplateSettingsController_Edit_ReturnsAViewResult()
        {
            var result = _notificationTemplateSettingsController.Edit();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void NotificationTemplateSettingsController_Edit_ShouldReturnTheResultOfTheGetMethod()
        {
            var notificationTemplateSettings = new MessageTemplateSettings();
            A.CallTo(() => _notificationTemplateSettingsManager.Get()).Returns(notificationTemplateSettings);
            
            var result = _notificationTemplateSettingsController.Edit();

            result.Model.Should().Be(notificationTemplateSettings);
        }

        [Fact]
        public void NotificationTemplateSettingsController_EditPost_ShouldCallManagersSaveMethodWithPassedObject()
        {
            var notificationTemplateSettings = new MessageTemplateSettings();

            _notificationTemplateSettingsController.Edit_POST(notificationTemplateSettings);

            A.CallTo(() => _notificationTemplateSettingsManager.Save(notificationTemplateSettings)).MustHaveHappened();
        }

        [Fact]
        public void NotificationTemplateSettingsController_EditPost_ShouldRedirectToEdit()
        {
            var notificationTemplateSettings = new MessageTemplateSettings();

            var result = _notificationTemplateSettingsController.Edit_POST(notificationTemplateSettings);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.RouteValues["action"].Should().Be("Edit");
        }
    }
}