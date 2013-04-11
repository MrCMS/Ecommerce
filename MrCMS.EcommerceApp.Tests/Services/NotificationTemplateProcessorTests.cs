using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class NotificationTemplateProcessorTests
    {
        private readonly NotificationTemplateProcesor _notificationTemplateProcessor;

        public NotificationTemplateProcessorTests()
        {
            _notificationTemplateProcessor = new NotificationTemplateProcesor();
        }

        [Fact]
        public void NotificationTemplateProcesor_ReplaceTokens_ShouldReplaceAutoProperty()
        {
            var user = new TestUser { FirstName = "John" };
            string template = "Hello {FirstName}";

            string processedTemplate = _notificationTemplateProcessor.ReplaceTokens(user, template);

            processedTemplate.Should().Be("Hello John");
        }

        [Fact]
        public void NotificationTemplateProcesor_ReplaceTokens_ShouldReplaceReadOnlyProperty()
        {
            var user = new TestUser { FirstName = "John", LastName = "Doe"};
            string template = "Hello {Name}";

            string processedTemplate = _notificationTemplateProcessor.ReplaceTokens(user, template);

            processedTemplate.Should().Be("Hello John Doe");
        }

        [Fact]
        public void NotificationTemplateProcesor_ReplaceMethods_ReplacesParameterlessMethods()
        {
            var user = new TestUser { FirstName = "John", LastName = "Doe" };
            string template = "Your Name is {GetNameReversed()}";

            string processedTemplate = _notificationTemplateProcessor.ReplaceMethods(user, template);

            processedTemplate.Should().Be("Your Name is eoD nhoJ");
        }

        [Fact]
        public void NotificationTemplateProcesor_ReplaceExtensionMethods_ShouldFindAndReplaceTokensWithMethods()
        {
            var user = new TestUser { FirstName = "John", LastName = "Doe" };

            string template = "Your initials are {GetInitials()}";

            string processedTemplate = _notificationTemplateProcessor.ReplaceExtensionMethods(user, template);

            processedTemplate.Should().Be("Your initials are J.D");
        }
    }

    public class TestUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get { return FirstName + " " + LastName; } }
        public string GetNameReversed()
        {
            IEnumerable<char> nameReversed = Name.Reverse();
            return new string(nameReversed.ToArray());
        }
    }

    public static class TestUserExtender
    {
        public static string GetInitials(this TestUser user)
        {
            return string.Format("{0}.{1}", user.FirstName[0], user.LastName[0]);
        }
    }
}