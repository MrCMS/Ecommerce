using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class TaxTests
    {
        [Fact]
        public void ShouldBeTheItemTaxPlusTheShippingTax()
        {
            var model = new TestableCartModel(itemTax: 10m, shippingTax: 5m);

            model.Tax.Should().Be(15);
        }
    }
}