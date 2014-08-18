using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class GiftCardAmountTests
    {
        [Fact]
        public void ShouldBeZeroIfNoGiftCardsAreApplied()
        {
            var cartModel = new CartModel
            {
                AppliedGiftCards = new List<GiftCard>()
            };

            cartModel.GiftCardAmount.Should().Be(0m);
        }

        [Fact]
        public void ShouldBeTheSumOfTheAvailableAmountOfTheGiftCardsApplied()
        {
            var testableCartModel = new TestableCartModel(total: 100m)
            {
                AppliedGiftCards = new List<GiftCard>
                {
                    new GiftCardBuilder().WithAvailableAmount(10m).Build(),
                    new GiftCardBuilder().WithAvailableAmount(20m).Build(),
                    new GiftCardBuilder().WithAvailableAmount(30m).Build()
                }
            };

            testableCartModel.GiftCardAmount.Should().Be(60m);
        }

        [Fact]
        public void ShouldBeTheTotalIfTheSumOfTheAvailableAmountIsGreaterThanTheTotal()
        {
            var testableCartModel = new TestableCartModel(total: 50m)
            {
                AppliedGiftCards = new List<GiftCard>
                {
                    new GiftCardBuilder().WithAvailableAmount(10m).Build(),
                    new GiftCardBuilder().WithAvailableAmount(20m).Build(),
                    new GiftCardBuilder().WithAvailableAmount(30m).Build()
                }
            };

            testableCartModel.GiftCardAmount.Should().Be(50m);
        }
    }
}