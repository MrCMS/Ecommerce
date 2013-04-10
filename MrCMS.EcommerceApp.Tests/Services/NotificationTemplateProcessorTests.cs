using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;
using FakeItEasy;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class NotificationTemplateProcessorTests : InMemoryDatabaseTest
    {
        private INotificationTemplateProcessor _notificationTemplateProcessor;

        [Fact]
        public void NotificationTemplateProcesor_ReplaceTokens()
        {
            
        }
    }
}