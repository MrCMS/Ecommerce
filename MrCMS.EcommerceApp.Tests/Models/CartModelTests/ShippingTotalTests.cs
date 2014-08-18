using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class ShippingTotalTests
    {
        [Fact]
        public void IfShippingMethodIsNullShouldBeZero()
        {
            var cartModel = new CartModel {ShippingMethod = null};

            cartModel.ShippingTotal.Should().Be(0m);
        }

        [Fact]
        public void IfShippingMethodIsNotNullShouldBeTheValueOfItsGetShippingTotalMethod()
        {
            var shippingMethod = A.Fake<IShippingMethod>();
            var cartModel = new CartModel {ShippingMethod = shippingMethod};
            A.CallTo(() => shippingMethod.GetShippingTotal(cartModel)).Returns(1.23m);

            cartModel.ShippingTotal.Should().Be(1.23m);
        }
    }
}