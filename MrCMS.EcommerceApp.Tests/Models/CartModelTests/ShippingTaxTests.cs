using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class ShippingTaxTests
    {
        [Fact]
        public void IfShippingMethodIsNullShouldBeZero()
        {
            var cartModel = new CartModel {ShippingMethod = null};

            cartModel.ShippingTax.Should().Be(0m);
        }

        [Fact]
        public void IfShippingMethodIsNotNullShouldBeTheValueOfItsGetShippingTaxMethod()
        {
            var shippingMethod = A.Fake<IShippingMethod>();
            var cartModel = new CartModel {ShippingMethod = shippingMethod};
            A.CallTo(() => shippingMethod.GetShippingTax(cartModel)).Returns(1.23m);

            cartModel.ShippingTax.Should().Be(1.23m);
        }
    }
}