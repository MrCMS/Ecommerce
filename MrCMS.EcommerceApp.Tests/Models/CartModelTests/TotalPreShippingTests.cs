using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class TotalPreShippingTests
    {
        [Fact]
        public void ShouldBeTheSumOfTheItemsPriceLessTheOrderTotalDiscount()
        {
            var model = new TestableCartModel(orderTotalDiscount: 10m)
            {
                Items = new List<CartItem>
                {
                    new CartItemBuilder().WithPricePreTax(10).WithTax(5).Build(),
                    new CartItemBuilder().WithPricePreTax(20).WithTax(5).Build(),
                    new CartItemBuilder().WithPricePreTax(30).WithTax(5).Build()
                }
            };

            model.TotalPreShipping.Should().Be(65);
        }
    }
}