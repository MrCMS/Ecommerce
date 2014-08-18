using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class DiscountAmountTests
    {
        [Fact]
        public void ShouldBeOrderTotalDiscountPlusItemTotalIfThatAmountIsLessThanTHeOrderTotalPreDiscount()
        {
            var cartModel = new TestableCartModel(totalPreDiscount: 20m, orderTotalDiscount: 10m, itemDiscount: 5m);

            cartModel.DiscountAmount.Should().Be(15m);
        }

        [Fact]
        public void ShouldBeOrderTotalPreDiscountIfThatValueIsLessThanOrderTotalDiscountPlusItemTotal()
        {
            var cartModel = new TestableCartModel(totalPreDiscount: 10m, orderTotalDiscount: 10m, itemDiscount: 5m);

            cartModel.DiscountAmount.Should().Be(10m);
        }
    }
}