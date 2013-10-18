using FluentAssertions;
using Xunit;

namespace MrCMS.AmazonApp.Tests
{
    public class AmazonAppTests
    {
        private readonly Web.Apps.Amazon.AmazonApp _amazonApp;

        public AmazonAppTests()
        {
            _amazonApp = new Web.Apps.Amazon.AmazonApp();
        }

        [Fact]
        public void AmazonApp_AppName_ShouldBeAmazon()
        {
            _amazonApp.AppName.Should().Be("Amazon");
        }
    }
}