using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class TotalTests
    {
        [Fact]
        public void ShouldBeTheTotalPreShippingPlusTheShippingTotal()
        {
            var model = new TestableCartModel(totalPreShipping: 10m, shippingTotal: 5m);

            model.Total.Should().Be(15);
        }
    }
}