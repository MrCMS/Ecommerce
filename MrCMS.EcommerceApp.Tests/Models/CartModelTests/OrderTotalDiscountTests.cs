using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class OrderTotalDiscountTests
    {
        [Fact]
        public void IfDiscountIsNullShouldBeZero()
        {
            var cartModel = new CartModel {Discount = null};

            cartModel.OrderTotalDiscount.Should().Be(decimal.Zero);
        }

        [Fact]
        public void IfDiscountIsSetShouldBeAmountOfGetDiscount()
        {
            var discount = A.Fake<Discount>();
            var cartModel = new CartModel {Discount = discount};
            A.CallTo(() => discount.GetDiscount(cartModel)).Returns(1.23m);

            cartModel.OrderTotalDiscount.Should().Be(1.23m);
        }
    }
}