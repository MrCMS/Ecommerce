using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class ShippingPreTaxTests
    {
        [Fact]
        public void ShouldBeShippingTotalLessTax()
        {
            var cartModel = new TestableCartModel(shippingTotal: 10m, shippingTax: 1m);

            cartModel.ShippingPreTax.Should().Be(9m);
        }
    }
}