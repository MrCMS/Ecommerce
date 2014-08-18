using System.Collections.Generic;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.Orders
{
    public class GiftCardApplierTests
    {
        [Fact]
        public void IfNoGiftCardsAppliedNoUsagesShouldBeCreated()
        {
            var order = new Order();
            GiftCardApplier giftCardApplier = new GiftCardApplierBuilder().Build();

            order = giftCardApplier.Apply(new List<GiftCard>(), order);

            order.GiftCardUsages.Should().BeEmpty();
        }

        [Fact]
        public void IfAGiftCardWithLessValueThanTheTotalIsAddedItShouldBeUsedInFull()
        {
            var order = new Order {Total = 50};
            var giftCard = new GiftCard {Value = 25};
            var giftCards = new List<GiftCard> {giftCard};
            GiftCardApplier giftCardApplier = new GiftCardApplierBuilder().Build();

            order = giftCardApplier.Apply(giftCards, order);

            order.GiftCardUsages.Should().HaveCount(1);
            order.GiftCardUsages[0].Amount.Should().Be(25);
            giftCard.GiftCardUsages.Should().HaveCount(1);
            giftCard.GiftCardUsages[0].Should().Be(order.GiftCardUsages[0]);
        }

        [Fact]
        public void MultipleCardsShouldBeAppliedInFullIfTotalIsLessThanTheTotal()
        {
            var order = new Order {Total = 50};
            var giftCard1 = new GiftCard {Value = 12};
            var giftCard2 = new GiftCard {Value = 13};
            var giftCards = new List<GiftCard> {giftCard1, giftCard2};
            GiftCardApplier giftCardApplier = new GiftCardApplierBuilder().Build();

            order = giftCardApplier.Apply(giftCards, order);

            order.GiftCardUsages.Should().HaveCount(2);
            order.GiftCardUsages[0].Amount.Should().Be(12);
            order.GiftCardUsages[1].Amount.Should().Be(13);
            giftCard1.GiftCardUsages.Should().HaveCount(1);
            giftCard2.GiftCardUsages.Should().HaveCount(1);
        }

        [Fact]
        public void IfTheCardValueIsGreaterThanTheTotalOnlyTheTotalAmountShouldBeApplied()
        {
            var order = new Order {Total = 50};
            var giftCard1 = new GiftCard {Value = 100};
            var giftCards = new List<GiftCard> {giftCard1};
            GiftCardApplier giftCardApplier = new GiftCardApplierBuilder().Build();

            order = giftCardApplier.Apply(giftCards, order);

            order.GiftCardUsages.Should().HaveCount(1);
            order.GiftCardUsages[0].Amount.Should().Be(50);
        }

        [Fact]
        public void IfASecondCardTakesItOverTheThresholdTheFullAmountShouldBeDrawnFromTheFirstThenTheRemainerFromTheSecond()
        {
            var order = new Order {Total = 50};
            var giftCard1 = new GiftCard {Value = 10};
            var giftCard2 = new GiftCard {Value = 100};
            var giftCards = new List<GiftCard> {giftCard1,giftCard2};
            GiftCardApplier giftCardApplier = new GiftCardApplierBuilder().Build();

            order = giftCardApplier.Apply(giftCards, order);

            order.GiftCardUsages.Should().HaveCount(2);
            order.GiftCardUsages[0].Amount.Should().Be(10);
            order.GiftCardUsages[1].Amount.Should().Be(40);
            giftCard1.AvailableAmount.Should().Be(0);
            giftCard2.AvailableAmount.Should().Be(60);
        }
    }
}