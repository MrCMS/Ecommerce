using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models.CartModelTests
{
    public class TotalToPayTests
    {
        [Fact]
        public void ShouldBeTheTotalIfThereAreNoGiftCardsApplied()
        {
            var testableCartModel = new TestableCartModel(total: 100m)
            {
                AppliedGiftCards = new List<GiftCard>()
            };

            testableCartModel.TotalToPay.Should().Be(100m);
        }

        [Fact]
        public void ShouldBeTheTotalLessTheAppliedGiftCardAvailableAmount()
        {
            var testableCartModel = new TestableCartModel(total: 100m, giftCardAmount:60m);

            testableCartModel.TotalToPay.Should().Be(40m);
        }
    }
}