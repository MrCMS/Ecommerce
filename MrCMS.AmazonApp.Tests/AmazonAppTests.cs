using FluentAssertions;
using Xunit;

namespace MrCMS.AmazonApp.Tests
{
    public class AmazonAppTests
    {
        private Web.Apps.Amazon.AmazonApp _amazonApp;

        public AmazonAppTests()
        {
            _amazonApp = new Web.Apps.Amazon.AmazonApp();
        }

        [Fact]
        public void EcommerceApp_AppName_ShouldBeAmazon()
        {
            _amazonApp.AppName.Should().Be("Amazon");
        }
    }
}