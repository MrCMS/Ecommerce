using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class SubtotalTests
    {
        [Fact]
        public void ShouldBeTheSumOfTheItemsPricePreTax()
        {
            var model = new CartModel()
            {
                Items = new List<CartItemData>
                {
                    new CartItemBuilder().WithPricePreTax(10).Build(),
                    new CartItemBuilder().WithPricePreTax(20).Build(),
                    new CartItemBuilder().WithPricePreTax(30).Build()
                }
            };

            model.Subtotal.Should().Be(60);
        }
    }
}