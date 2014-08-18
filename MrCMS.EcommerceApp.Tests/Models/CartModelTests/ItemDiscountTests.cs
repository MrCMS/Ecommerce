using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class ItemDiscountTests
    {
        [Fact]
        public void IfDiscountIsNullShouldBeZero()
        {
            var cartModel = new CartModel { Discount = null };

            cartModel.ItemDiscount.Should().Be(decimal.Zero);
        }

        [Fact]
        public void IfDiscountIsSetShouldBeAmountOfGetDiscount()
        {
            var discount = A.Fake<Discount>();
            var cartItem = new CartItemBuilder().WithPricePreTax(10).WithTax(5).Build();

            var model = new CartModel
            {
                Discount = discount,
                Items = new List<CartItem>
                {
                    cartItem
                }
            };
            // this method is called in the builder and allows the info to be set
            model.Items.ForEach(item => item.SetDiscountInfo(model));
            A.CallTo(() => discount.GetDiscount(cartItem, model.DiscountCode)).Returns(1.23m);

            model.ItemDiscount.Should().Be(1.23m);
        }
    }
}