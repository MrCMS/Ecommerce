using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.Pricing
{
    public class ProductPricingMethodIndividuallyPricedDiscountTests
    {
        private const decimal TaxPercentage = 20;

        [Fact]
        public void PriceTest1()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.8);
        }

        [Fact]
        public void PriceTest2()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 5);
        }

        [Fact]
        public void PriceTest3()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.8);
        }

        [Fact]
        public void PriceTest4()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5);
        }

        [Fact]
        public void PriceTest5()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.8);
        }

        [Fact]
        public void PriceTest6()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 5);
        }

        [Fact]
        public void PriceTest7()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.8);
        }

        [Fact]
        public void PriceTest8()
        {
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5);
        }

        private void DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(double basePrice,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder()
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .CalculateTaxOnIndividualItem()
                    .WithDiscountOnPrices(discountOnPrices)
                    .WithCustomerTaxApplication(applyCustomerTax)
                    .Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountAmount(1).Build();

            //check for various quantities - should be fine with individual item pricing to just multiply up
            for (int quantity = 1; quantity <= 10; quantity++)
            {
                cartItemData.Quantity = quantity;
                pricingMethod.GetPrice(cartItemData).Should().Be(Convert.ToDecimal(expected*quantity));
            }
        }

        [Fact]
        public void TaxTest1()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 0.8);
        }

        [Fact]
        public void TaxTest2()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 0.83);
        }

        [Fact]
        public void TaxTest3()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        [Fact]
        public void TaxTest4()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        [Fact]
        public void TaxTest5()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 0.8);
        }

        [Fact]
        public void TaxTest6()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 0.83);
        }

        [Fact]
        public void TaxTest7()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        [Fact]
        public void TaxTest8()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        private void DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(double basePrice,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder()
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .CalculateTaxOnIndividualItem()
                    .WithDiscountOnPrices(discountOnPrices)
                    .WithCustomerTaxApplication(applyCustomerTax)
                    .Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountAmount(1).Build();

            //check for various quantities - should be fine with individual item pricing to just multiply up
            for (int quantity = 1; quantity <= 10; quantity++)
            {
                cartItemData.Quantity = quantity;
                pricingMethod.GetTax(cartItemData).Should().Be(Convert.ToDecimal(expected*quantity));
            }
        }

        [Fact]
        public void PricePreTaxTest1()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4);
        }

        [Fact]
        public void PricePreTaxTest2()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.17);
        }

        [Fact]
        public void PricePreTaxTest3()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 3.8);
        }

        [Fact]
        public void PricePreTaxTest4()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4);
        }

        [Fact]
        public void PricePreTaxTest5()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4);
        }

        [Fact]
        public void PricePreTaxTest6()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.17);
        }

        [Fact]
        public void PricePreTaxTest7()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 3.8);
        }

        [Fact]
        public void PricePreTaxTest8()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4);
        }

        private void DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(decimal basePrice,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().WithPriceLoadingMethod(priceLoadingMethod)
                    .CalculateTaxOnIndividualItem()
                    .WithDiscountOnPrices(discountOnPrices)
                    .WithCustomerTaxApplication(applyCustomerTax)
                    .Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(basePrice)
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountAmount(1).Build();

            //check for various quantities - should be fine with individual item pricing to just multiply up
            for (int quantity = 1; quantity <= 10; quantity++)
            {
                cartItemData.Quantity = quantity;
                pricingMethod.GetPricePreTax(cartItemData).Should().Be(Convert.ToDecimal(expected*quantity));
            }
        }
    }
}