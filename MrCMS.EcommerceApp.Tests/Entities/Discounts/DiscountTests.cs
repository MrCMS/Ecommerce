using System;
using FakeItEasy;
using FluentAssertions;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Discounts
{
    public class DiscountTests
    {
        [Fact]
        public void Discount_IsCodeValid_IfCodeDoesNotMatchReturnsFalse()
        {
            var discount = new Discount { Code = "test-code" };

            var isCodeValid = discount.IsCodeValid("different-code");

            isCodeValid.Should().BeFalse();
        }

        [Fact]
        public void Discount_IsCodeValid_IfCodeMatchesButHasNotStartedYetReturnFalse()
        {
            var discount = new Discount { Code = "test-code", DateFrom = DateTime.Now.AddDays(1) };

            var isCodeValid = discount.IsCodeValid("test-code");

            isCodeValid.Should().BeFalse();
        }

        [Fact]
        public void Discount_IsCodeValid_IfCodeMatchesButHasFinishedReturnFalse()
        {
            var discount = new Discount { Code = "test-code", DateTo = DateTime.Now.AddDays(-1) };

            var isCodeValid = discount.IsCodeValid("test-code");

            isCodeValid.Should().BeFalse();
        }

        [Fact]
        public void Discount_IsCodeValid_IfCodeMatchesAndHasNoDatesShouldBeTrue()
        {
            var discount = new Discount { Code = "test-code" };

            var isCodeValid = discount.IsCodeValid("test-code");

            isCodeValid.Should().BeTrue();
        }

        [Fact]
        public void Discount_IsCodeValid_IfCodeMatchesIsWithinDatesShouldReturnTrue()
        {
            var discount = new Discount
                               {
                                   Code = "test-code",
                                   DateFrom = DateTime.Now.AddDays(-1),
                                   DateTo = DateTime.Now.AddDays(1)
                               };

            var isCodeValid = discount.IsCodeValid("test-code");

            isCodeValid.Should().BeTrue();
        }

        [Fact]
        public void Discount_GetDiscountForModel_IfCodeIsInvalidReturnZero()
        {
            var discount = new TestableDiscount { OverridenValidity = false };
            var cartModel = new CartModel();

            var amount = discount.GetDiscount(cartModel);
            amount.Should().Be(0);
        }

        [Fact]
        public void Discount_GetDiscountForItem_IfCodeIsInvalidReturnZero()
        {
            var discount = new TestableDiscount { OverridenValidity = false };
            var cartItem = new CartItem();

            var amount = discount.GetDiscount(cartItem, "code");

            amount.Should().Be(0);
        }

        [Fact]
        public void Discount_GetDiscountForModel_IfLimitationExcludesModelReturnZero()
        {
            var limitation = A.Fake<DiscountLimitation>();
            var discount = new TestableDiscount { OverridenValidity = true, Limitation = limitation };
            var cartModel = new CartModel();
            A.CallTo(() => limitation.IsCartValid(cartModel)).Returns(false);

            var amount = discount.GetDiscount(cartModel);

            amount.Should().Be(0);
        }

        [Fact]
        public void Discount_GetDiscountForItem_IfLimitationExcludesItemReturnZero()
        {
            var limitation = A.Fake<DiscountLimitation>();
            var discount = new TestableDiscount { OverridenValidity = true, Limitation = limitation };
            var cartItem = new CartItem();
            A.CallTo(() => limitation.IsItemValid(cartItem)).Returns(false);

            var amount = discount.GetDiscount(cartItem, "code");

            amount.Should().Be(0);
        }

        [Fact]
        public void Discount_GetDiscountForModel_IfApplicationIsNullReturnZero()
        {
            var discount = new TestableDiscount {OverridenValidity = true};
            var cartModel = new CartModel();

            var amount = discount.GetDiscount(cartModel);

            amount.Should().Be(0);
        }

        [Fact]
        public void Discount_GetDiscountForItem_IfApplicationIsNullReturnZero()
        {
            var discount = new TestableDiscount {OverridenValidity = true};
            var cartItem = new CartItem();

            var amount = discount.GetDiscount(cartItem, "code");

            amount.Should().Be(0);
        }

        [Fact]
        public void Discount_GetDiscountForModel_IfApplicationIsSetAndIsValidReturnTheResultOfGetDiscountAmount()
        {
            var discountApplication = A.Fake<DiscountApplication>();
            var discount = new TestableDiscount {OverridenValidity = true,Application = discountApplication};
            var cartModel = new CartModel();
            A.CallTo(() => discountApplication.GetDiscount(cartModel)).Returns(123);

            var amount = discount.GetDiscount(cartModel);

            amount.Should().Be(123);
        }

        [Fact]
        public void Discount_GetDiscountForItem_IfApplicationIsSetAndIsValidReturnTheResultOfGetDiscountAmount()
        {
            var discountApplication = A.Fake<DiscountApplication>();
            var discount = new TestableDiscount {OverridenValidity = true,Application = discountApplication};
            var cartItem = new CartItem();
            A.CallTo(() => discountApplication.GetDiscount(cartItem)).Returns(123);

            var amount = discount.GetDiscount(cartItem, "code");

            amount.Should().Be(123);
        }
    }

    [DoNotMap]
    public class TestableDiscount : Discount
    {
        public override bool IsCodeValid(string discountCode)
        {
            return OverridenValidity.GetValueOrDefault(base.IsCodeValid(discountCode));
        }

        public bool? OverridenValidity { get; set; }
    }
}