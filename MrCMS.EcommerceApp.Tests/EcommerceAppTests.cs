using FluentAssertions;
using Xunit;

namespace MrCMS.EcommerceApp.Tests
{
    public class EcommerceAppTests
    {
        private readonly Web.Apps.Ecommerce.EcommerceApp _ecommerceApp;

        public EcommerceAppTests()
        {
            _ecommerceApp = new Web.Apps.Ecommerce.EcommerceApp();
        }

        [Fact]
        public void EcommerceApp_AppName_ShouldBeEcommerce()
        {
            _ecommerceApp.AppName.Should().Be("Ecommerce");
        }
    }
}