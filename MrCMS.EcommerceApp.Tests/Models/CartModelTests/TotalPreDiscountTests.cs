using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class TotalPreDiscountTests
    {
        [Fact]
        public void ShouldBeTheSumOfTheItemsPriceIncludingTax()
        {
            var model = new CartModel
            {
                Items = new List<CartItemData>
                {
                    new CartItemBuilder().WithPricePreTax(10).WithTax(5).Build(),
                    new CartItemBuilder().WithPricePreTax(20).WithTax(5).Build(),
                    new CartItemBuilder().WithPricePreTax(30).WithTax(5).Build()
                }
            };

            model.TotalPreDiscount.Should().Be(75);
        }
    }
}