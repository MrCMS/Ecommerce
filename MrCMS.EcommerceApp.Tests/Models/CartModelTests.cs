using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models
{
    public class CartModelTests
    {
        [Fact]
        public void CartModel_SubTotal_ShouldBeTheSumOfPricePreTax()
        {
            var cartItem1 = A.Fake<CartItem>();
            var cartItem2 = A.Fake<CartItem>();
            A.CallTo(() => cartItem1.PricePreTax).Returns(10);
            A.CallTo(() => cartItem2.PricePreTax).Returns(20);
            var cartModel = new CartModel
                                {
                                    Items = new List<CartItem> { cartItem1, cartItem2 }
                                };

            var subtotal = cartModel.Subtotal;

            subtotal.Should().Be(30);
        }

        [Fact]
        public void CartModel_SubTotal_NoItemsShouldBeZero()
        {
            var cartModel = new CartModel
                                {
                                    Items = new List<CartItem>()
                                };

            var subtotal = cartModel.Subtotal;

            subtotal.Should().Be(0);
        }

        [Fact]
        public void CartModel_TotalPreDiscount_ShouldBeTheSumOfPrice()
        {
            var cartItem1 = A.Fake<CartItem>();
            var cartItem2 = A.Fake<CartItem>();
            A.CallTo(() => cartItem1.Price).Returns(10);
            A.CallTo(() => cartItem2.Price).Returns(20);
            var cartModel = new CartModel
            {
                Items = new List<CartItem> { cartItem1, cartItem2 }
            };

            var total = cartModel.TotalPreDiscount;

            total.Should().Be(30);
        }

        [Fact]
        public void CartModel_TaxRates_ShouldBreakDownTotalsByTaxRatePercentage()
        {
            var cartItem1 = A.Fake<CartItem>();
            var cartItem2 = A.Fake<CartItem>();
            A.CallTo(() => cartItem1.TaxRatePercentage).Returns(0);
            A.CallTo(() => cartItem1.Price).Returns(10);
            A.CallTo(() => cartItem2.TaxRatePercentage).Returns(20);
            A.CallTo(() => cartItem2.Price).Returns(20);
            var cartModel = new CartModel
            {
                Items = new List<CartItem> { cartItem1, cartItem2 }
            };

            var dictionary = cartModel.TaxRates;

            dictionary[0].Should().Be(10);
            dictionary[20].Should().Be(20);
        }

        [Fact]
        public void CartModel_DiscountAmount_ShouldBe0IfDiscountIsNull()
        {
            var cartModel = new CartModel();

            var discountAmount = cartModel.DiscountAmount;

            discountAmount.Should().Be(0);
        }

        [Fact]
        public void CartModel_DiscountAmount_ShouldBeReturnValueOfGetDiscountAmount()
        {
            var cartItem1 = A.Fake<CartItem>();
            A.CallTo(() => cartItem1.Price).Returns(20);
            var discount = A.Fake<Discount>();
            var cartModel = new CartModel
            {
                Items = new List<CartItem> { cartItem1 },
                Discount = discount
            };
            A.CallTo(() => discount.GetDiscount(cartModel)).Returns(10);

            var discountAmount = cartModel.DiscountAmount;

            discountAmount.Should().Be(10);
        }

        [Fact]
        public void CartModel_Total_WithDiscountShouldBeReduced()
        {
            var cartItem1 = A.Fake<CartItem>();
            var cartItem2 = A.Fake<CartItem>();
            A.CallTo(() => cartItem1.Price).Returns(10);
            A.CallTo(() => cartItem2.Price).Returns(20);
            var discount = A.Fake<Discount>();
            var cartModel = new CartModel
            {
                Items = new List<CartItem> { cartItem1, cartItem2 },
                Discount = discount
            };
            A.CallTo(() => discount.GetDiscount(cartModel)).Returns(10);

            var total = cartModel.Total;

            total.Should().Be(20);
        }

        [Fact]
        public void CartModel_Tax_ShouldBeTheSumOfTax()
        {
            var cartItem1 = A.Fake<CartItem>();
            var cartItem2 = A.Fake<CartItem>();
            A.CallTo(() => cartItem1.Tax).Returns(10);
            A.CallTo(() => cartItem2.Tax).Returns(20);
            var cartModel = new CartModel
            {
                Items = new List<CartItem> { cartItem1, cartItem2 }
            };

            var tax = cartModel.Tax;

            tax.Should().Be(30);
        }

        [Fact]
        public void CartModel_Tax_EmptyCartShouldBeZero()
        {
            var cartModel = new CartModel
            {
                Items = new List<CartItem>()
            };

            var total = cartModel.Tax;

            total.Should().Be(0);
        }

        [Fact]
        public void CartModel_DiscountAmount_IfItIsCalculatedAsGreaterThanThePreDiscountTotalReturnThat()
        {
            var cartItem1 = A.Fake<CartItem>();
            A.CallTo(() => cartItem1.Price).Returns(5);
            var discount = A.Fake<Discount>();
            var cartModel = new CartModel
            {
                Items = new List<CartItem> { cartItem1 },
                Discount = discount
            };
            A.CallTo(() => discount.GetDiscount(cartModel)).Returns(10);

            var discountAmount = cartModel.DiscountAmount;

            discountAmount.Should().Be(5);
        }

        [Fact]
        public void CartModel_CanCheckout_IfAllItemsCanBeBoughtThenReturnsTrue()
        {
            var cartItem = new CartItemBuilder().CanBuy().Build();
            var cartModel = new CartModelBuilder().WithItems(cartItem).Build();

            var canCheckout = cartModel.CanCheckout;

            canCheckout.Should().BeTrue();
        }

        //[Fact]
        //public void CartModel_CanCheckout_IfAnyItemsCannotBeBoughtThenReturnsFalse()
        //{
        //    var cartItem1 = new CartItemBuilder().CanBuy().Build();
        //    var cartItem2 = new CartItemBuilder().CanBuy().Build();
        //    var cartItem3 = new CartItemBuilder().CannotBuy().Build();
        //    var cartModel = new CartModel
        //    {
        //        Items = new List<CartItem> { cartItem1, cartItem2, cartItem3 },
        //    };

        //    var canCheckout = cartModel.CanCheckout;

        //    canCheckout.Should().BeFalse();
        //}

        [Fact]
        public void CartModel_CanCheckout_NoItemsShouldBeFalse()
        {
            var cartModel = new CartModel();

            var canCheckout = cartModel.CanCheckout;

            canCheckout.Should().BeFalse();
        }
    }
}