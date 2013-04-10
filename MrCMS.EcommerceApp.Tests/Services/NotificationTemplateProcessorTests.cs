using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;
using FakeItEasy;
using MrCMS.Entities.People;
using System;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class NotificationTemplateProcessorTests : InMemoryDatabaseTest
    {
        private INotificationTemplateProcessor _notificationTemplateProcessor;
        private INotificationTemplateSettingsManager _notificationTemplateSettingsManager;

        public NotificationTemplateProcessorTests()
        {
            _notificationTemplateProcessor = new NotificationTemplateProcesor();
            _notificationTemplateSettingsManager = new NotificationTemplateSettingsManager(Session);
        }

        [Fact]
        public void NotificationTemplateProcesor_ReplaceTokens()
        {
            Session.Transact(session => session.Save(new User { FirstName = "John" }));
            User user = Session.QueryOver<User>().List().First();

            string template = "Hello {FirstName}";
            Session.Transact(session => session.Save(new NotificationTemplateSettings { OrderConfirmationTemplate=template}));
            NotificationTemplateSettings notificationTemplateSettings = Session.QueryOver<NotificationTemplateSettings>().List().First();

            string processedTemplate = _notificationTemplateProcessor.ReplaceTokens<User>(user, notificationTemplateSettings.OrderConfirmationTemplate);

            Assert.Contains( "John",processedTemplate);
        }

        [Fact]
        public void NotificationTemplateProcesor_ReplaceMethods()
        {
            Session.Transact(session => session.Save(new User { FirstName = "John", LastName="Doe"}));
            User user = Session.QueryOver<User>().List().First();

            string template = "Your Name is {get_FirstName()}";
            Session.Transact(session => session.Save(new NotificationTemplateSettings { OrderConfirmationTemplate = template }));
            NotificationTemplateSettings notificationTemplateSettings = Session.QueryOver<NotificationTemplateSettings>().List().First();

            string processedTemplate = _notificationTemplateProcessor.ReplaceMethods<User>(user, notificationTemplateSettings.OrderConfirmationTemplate);

            Assert.Contains("John", processedTemplate);
        }

        [Fact]
        public void NotificationTemplateProcesor_ReplaceExtensionMethods()
        {
            Session.Transact(session => session.Save(new User { FirstName = "John", LastName = "Doe" }));
            User user = Session.QueryOver<User>().List().First();

            string template = "Your Name is {GetFirstAndLastName()}";
            Session.Transact(session => session.Save(new NotificationTemplateSettings { OrderConfirmationTemplate = template }));
            NotificationTemplateSettings notificationTemplateSettings = Session.QueryOver<NotificationTemplateSettings>().List().First();

            string processedTemplate = _notificationTemplateProcessor.ReplaceExtensionMethods<User>(user, notificationTemplateSettings.OrderConfirmationTemplate);

            Assert.Contains("John Doe", processedTemplate);
        }
    }
}