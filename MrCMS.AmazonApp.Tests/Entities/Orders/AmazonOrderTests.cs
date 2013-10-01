using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Entities.Orders
{
    public class AmazonOrderTests
    {
        [Fact]
        public void AmazonOrder_ItemDiscountAmount_ShouldBeSumOfPromotionDiscountAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.PromotionDiscountAmount).Returns(10);
            A.CallTo(() => item2.PromotionDiscountAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
                {
                    Items = new List<AmazonOrderItem> {item1, item2}
                };

            var result = amazonOrder.ItemDiscountAmount;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_ItemTax_ShouldBeSumOfItemTaxAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.ItemTaxAmount).Returns(10);
            A.CallTo(() => item2.ItemTaxAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1, item2 }
            };

            var result = amazonOrder.ItemTax;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_ItemAmount_ShouldBeSumOfItemPriceAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.ItemPriceAmount).Returns(10);
            A.CallTo(() => item2.ItemPriceAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1, item2 }
            };

            var result = amazonOrder.ItemAmount;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_ShippingAmount_ShouldBeSumOfShippingPriceAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.ShippingPriceAmount).Returns(10);
            A.CallTo(() => item2.ShippingPriceAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1, item2 }
            };

            var result = amazonOrder.ShippingAmount;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_ShippingTax_ShouldBeSumOfShippingTaxAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.ShippingTaxAmount).Returns(10);
            A.CallTo(() => item2.ShippingTaxAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1, item2 }
            };

            var result = amazonOrder.ShippingTax;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_ShippingDiscountAmount_ShouldBeSumOfShippingDiscountAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.ShippingDiscountAmount).Returns(10);
            A.CallTo(() => item2.ShippingDiscountAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1, item2 }
            };

            var result = amazonOrder.ShippingDiscountAmount;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_GiftWrapAmount_ShouldBeSumOfGiftWrapPriceAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.GiftWrapPriceAmount).Returns(10);
            A.CallTo(() => item2.GiftWrapPriceAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1, item2 }
            };

            var result = amazonOrder.GiftWrapAmount;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_GiftWrapTax_ShouldBeSumOfGiftWrapTaxAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            var item2 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.GiftWrapTaxAmount).Returns(10);
            A.CallTo(() => item2.GiftWrapTaxAmount).Returns(20);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1, item2 }
            };

            var result = amazonOrder.GiftWrapTax;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_Tax_ShouldBeSumOfItemTaxShippingTaxGiftWrapTax()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.ItemTaxAmount).Returns(10);
            A.CallTo(() => item1.ShippingTaxAmount).Returns(10);
            A.CallTo(() => item1.GiftWrapTaxAmount).Returns(10);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1 }
            };

            var result = amazonOrder.Tax;

            result.Should().Be(30);
        }

        [Fact]
        public void AmazonOrder_ShippingTotal_ShouldBeSumOShippingAmountShippingTaxMinusShippingDiscountAmount()
        {
            var item1 = A.Fake<AmazonOrderItem>();
            A.CallTo(() => item1.ShippingPriceAmount).Returns(10);
            A.CallTo(() => item1.ShippingTaxAmount).Returns(10);
            A.CallTo(() => item1.ShippingDiscountAmount).Returns(10);
            var amazonOrder = new AmazonOrder()
            {
                Items = new List<AmazonOrderItem> { item1 }
            };

            var result = amazonOrder.ShippingTotal;

            result.Should().Be(10);
        }
    }
}