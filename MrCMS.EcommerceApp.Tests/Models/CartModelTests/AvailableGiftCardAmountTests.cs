using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class AvailableGiftCardAmountTests
    {
        [Fact]
        public void ShouldBeZeroIfNoGiftCardsAreApplied()
        {
            var cartModel = new CartModel
            {
                AppliedGiftCards = new List<GiftCard>()
            };

            cartModel.AvailableGiftCardAmount.Should().Be(0m);
        }

        [Fact]
        public void ShouldBeTheSumOfTheAvailableAmountOfTheGiftCardsApplied()
        {
            var testableCartModel = new CartModel
            {
                AppliedGiftCards = new List<GiftCard>
                {
                    new GiftCardBuilder().WithAvailableAmount(10m).Build(),
                    new GiftCardBuilder().WithAvailableAmount(20m).Build(),
                    new GiftCardBuilder().WithAvailableAmount(30m).Build()
                }
            };

            testableCartModel.AvailableGiftCardAmount.Should().Be(60m);
        }
    }
}