using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Cart
{
    public class CartItemTests
    {
        [Fact]
        public void CartItem_PricePreTax_ShouldBeTheResultOfGetPricePreTax()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.GetPricePreTax(2)).Returns(20);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var pricePreTax = cartItem.PricePreTax;

            pricePreTax.Should().Be(20);
        }
        [Fact]
        public void CartItem_Price_ShouldBeTheResultOfGetPrice()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.GetPrice(2)).Returns(20);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var price = cartItem.Price;

            price.Should().Be(20);
        }

        [Fact]
        public void CartItem_Saving_ShouldBeTheResultOfGetSaving()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.GetSaving(2)).Returns(20);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var saving = cartItem.Saving;

            saving.Should().Be(20);
        }

        [Fact]
        public void CartItem_Tax_ShouldBeTheResultOfGetTax()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.GetTax(2)).Returns(20);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var tax = cartItem.Tax;

            tax.Should().Be(20);
        }

        [Fact]
        public void CartItem_CurrentlyAvailable_ShouldBeFalseIfStockLevelsAreTooLow()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.CanBuy(5)).Returns(false);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var currentlyAvailable = cartItem.CurrentlyAvailable;

            currentlyAvailable.Should().BeFalse();
        }

        [Fact]
        public void CartItem_CurrentlyAvailable_ShouldBeTrueIfProductIsAvailableForQuantity()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.CanBuy(2)).Returns(true);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var currentlyAvailable = cartItem.CurrentlyAvailable;

            currentlyAvailable.Should().BeTrue();
        }

        [Fact]
        public void CartItem_CurrentlyAvailable_ShouldBeTrueIfStockLevelsAreHighEnough()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.CanBuy(2)).Returns(true);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var currentlyAvailable = cartItem.CurrentlyAvailable;

            currentlyAvailable.Should().BeTrue();
        }

        [Fact]
        public void CartItem_TaxRatePercentage_ShouldReturnTheTaxRateInPercentage()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.TaxRatePercentage).Returns(20);
            var cartItem = new CartItem { Item = product, Quantity = 2 };

            var taxRatePercentage = cartItem.TaxRatePercentage;

            taxRatePercentage.Should().Be(20);
        }

        [Fact]
        public void CartItem_Weight_ShouldBeWeightTimesQuantity()
        {
            var product = A.Fake<IBuyableItem>();
            A.CallTo(() => product.Weight).Returns(123);
            var cartItem = new CartItem { Item = product, Quantity = 3 };

            var weight = cartItem.Weight;

            weight.Should().Be(369);
        }

        [Fact]
        public void CartItem_GetDiscountAmount_IfNullDiscountIsPassedShouldBeZero()
        {
            var product = A.Fake<IBuyableItem>();
            var cartItem = new CartItem { Item = product, Quantity = 3 };

            var discountAmount = cartItem.GetDiscountAmount(null, null);

            discountAmount.Should().Be(0);
        }

        [Fact]
        public void CartItem_GetDiscountAmount_IfValidDiscountIsPassedShouldReturnResultOfDiscountGetAmount()
        {
            var discount = A.Fake<Discount>();
            var product = A.Fake<IBuyableItem>();
            var cartItem = new CartItem { Item = product, Quantity = 3 };
            A.CallTo(() => discount.GetDiscount(cartItem, "test")).Returns(10m);

            var discountAmount = cartItem.GetDiscountAmount(discount, "test");

            discountAmount.Should().Be(10);
        }
    }
}