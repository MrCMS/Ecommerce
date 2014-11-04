using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class ShippingTaxPercentageTests
    {
        [Fact]
        public void IfShippingMethodIsNullShouldBeZero()
        {
            var cartModel = new CartModel{ShippingMethod = null};

            cartModel.ShippingTaxPercentage.Should().Be(0m);
        }

        [Fact]
        public void IfShippingMethodIsSetShouldBeValueOfItsShippingPercentage()
        {
            var shippingMethod = A.Fake<IShippingMethod>();
            var cartModel = new CartModel {ShippingMethod = shippingMethod};
            A.CallTo(() => shippingMethod.TaxRatePercentage).Returns(12.3m);

            cartModel.ShippingTaxPercentage.Should().Be(12.3m);
        }
    }
}