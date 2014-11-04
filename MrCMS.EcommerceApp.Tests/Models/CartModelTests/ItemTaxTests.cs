using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class ItemTaxTests
    {
        [Fact]
        public void ShouldBeTheSumOfTheItemTax()
        {
            var model = new CartModel
            {
                Items = new List<CartItem>
                {
                    new CartItemBuilder().WithPricePreTax(10).WithTax(5).Build(),
                    new CartItemBuilder().WithPricePreTax(20).WithTax(5).Build(),
                    new CartItemBuilder().WithPricePreTax(30).WithTax(5).Build()
                }
            };

            model.ItemTax.Should().Be(15);
        }
    }
}