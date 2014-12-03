using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities.Cart
{
    public class CartItemTests
    {
        private readonly ProductVariant _productVariant;

        public CartItemTests()
        {
            _productVariant = A.Fake<ProductVariant>();
        }


        [Fact]
        public void CartItem_Price_ShouldBeTheResultOfGetPrice()
        {
            A.CallTo(() => _productVariant.GetPrice(2)).Returns(20);
            var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

            var price = cartItem.Price;

            price.Should().Be(20);
        }

        [Fact]
        public void CartItem_Tax_ShouldBeResultOfGetTax()
        {
            A.CallTo(() => _productVariant.GetTax(2)).Returns(4);
            var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

            var tax = cartItem.Tax;

            tax.Should().Be(4);
        }

        [Fact]
        public void CartItem_PricePreTax_ShouldBePriceMinusTax()
        {
            A.CallTo(() => _productVariant.GetPrice(2)).Returns(24);
            A.CallTo(() => _productVariant.GetTax(2)).Returns(4);
            var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

            var pricePreTax = cartItem.PricePreTax;

            pricePreTax.Should().Be(20);
        }

        [Fact]
        public void CartItem_Saving_ShouldBeTheResultOfGetSaving()
        {
            A.CallTo(() => _productVariant.GetSaving(2)).Returns(20);
            var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

            var saving = cartItem.Saving;

            saving.Should().Be(20);
        }

        //[Fact]
        //public void CartItem_CurrentlyAvailable_ShouldBeFalseIfStockLevelsAreTooLow()
        //{

        //    A.CallTo(() => _productVariant.CanBuy(5)).Returns(new OutOfStock(_productVariant));
        //    var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

        //    var currentlyAvailable = cartItem.CurrentlyAvailable;

        //    currentlyAvailable.Should().BeFalse();
        //}

        //[Fact]
        //public void CartItem_CurrentlyAvailable_ShouldBeTrueIfProductIsAvailableForQuantity()
        //{
        //    A.CallTo(() => _productVariant.CanBuy(2)).Returns(new CanBuy());
        //    var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

        //    var currentlyAvailable = cartItem.CurrentlyAvailable;

        //    currentlyAvailable.Should().BeTrue();
        //}

        //[Fact]
        //public void CartItem_CurrentlyAvailable_ShouldBeTrueIfStockLevelsAreHighEnough()
        //{
        //    A.CallTo(() => _productVariant.CanBuy(2)).Returns(new CanBuy());
        //    var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

        //    var currentlyAvailable = cartItem.CurrentlyAvailable;

        //    currentlyAvailable.Should().BeTrue();
        //}

        [Fact]
        public void CartItem_TaxRatePercentage_ShouldReturnTheTaxRateInPercentage()
        {
            A.CallTo(() => _productVariant.TaxRatePercentage).Returns(20);
            var cartItem = new CartItem { Item = _productVariant, Quantity = 2 };

            var taxRatePercentage = cartItem.TaxRatePercentage;

            taxRatePercentage.Should().Be(20);
        }

        [Fact]
        public void CartItem_Weight_ShouldBeWeightTimesQuantity()
        {
            A.CallTo(() => _productVariant.Weight).Returns(123);
            var cartItem = new CartItem { Item = _productVariant, Quantity = 3 };

            var weight = cartItem.Weight;

            weight.Should().Be(369);
        }

        [Fact]
        public void CartItem_GetDiscountAmount_IfNullDiscountIsPassedShouldBeZero()
        {
            var cartItem = new CartItem { Item = _productVariant, Quantity = 3 };
            cartItem.SetDiscountInfo(0m);

            var discountAmount = cartItem.DiscountAmount;

            discountAmount.Should().Be(0);
        }
    }
}