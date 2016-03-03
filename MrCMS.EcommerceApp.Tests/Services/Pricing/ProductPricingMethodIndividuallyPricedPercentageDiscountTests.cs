using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.Pricing
{
    public class ProductPricingMethodIndividuallyPricedPercentageDiscountTests
    {
        private const decimal TaxPercentage = 20;

        [Fact]
        public void PriceTest1()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 5.4);
        }

        [Fact]
        public void PriceTest2()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 5.4);
        }

        [Fact]
        public void PriceTest3()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 5.4);
        }

        [Fact]
        public void PriceTest4()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5.4);
        }

        [Fact]
        public void PriceTest5()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 5.4);
        }

        [Fact]
        public void PriceTest6()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 5.4);
        }

        [Fact]
        public void PriceTest7()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 5.4);
        }

        [Fact]
        public void PriceTest8()
        {
            // 10% should always be 10%, so remove 10% from 6 so 0.6 - 6 - 0.6 = 5.4
            DiscountApplicationOnPriceIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5.4);
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
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountPercentage(10).Build();

            //check for various quantities - should be fine with individual item pricing to just multiply up
            for (int quantity = 1; quantity <= 10; quantity++)
            {
                cartItemData.Quantity = quantity;
                pricingMethod.GetPrice(cartItemData).Should().Be(Convert.ToDecimal(expected * quantity));
            }
        }

        [Fact]
        public void TaxTest1()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 0.9);
        }

        [Fact]
        public void TaxTest2()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 0.9);
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
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 0.9);
        }

        [Fact]
        public void TaxTest6()
        {
            DiscountApplicationOnTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 0.9);
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
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountPercentage(10).Build();

            //check for various quantities - should be fine with individual item pricing to just multiply up
            for (int quantity = 1; quantity <= 10; quantity++)
            {
                cartItemData.Quantity = quantity;
                pricingMethod.GetTax(cartItemData).Should().Be(Convert.ToDecimal(expected * quantity));
            }
        }

        [Fact]
        public void PricePreTaxTest1()
        {
            // 5.4 - 0.9 = 4.5
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.5);
        }

        [Fact]
        public void PricePreTaxTest2()
        {
            // 5.4 - 0.9 = 4.5
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.5);
        }

        [Fact]
        public void PricePreTaxTest3()
        {
            // 5.4 - 1 = 4.4
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.4);
        }

        [Fact]
        public void PricePreTaxTest4()
        {
            // 5.4 - 1 = 4.4
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4.4);
        }

        [Fact]
        public void PricePreTaxTest5()
        {
            // 5.4 - 0.9 = 4.5
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.5);
        }

        [Fact]
        public void PricePreTaxTest6()
        {
            // 5.4 - 0.9 = 4.5
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.5);
        }

        [Fact]
        public void PricePreTaxTest7()
        {
            // 5.4 - 1 = 4.4
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.4);
        }

        [Fact]
        public void PricePreTaxTest8()
        {
            // 5.4 - 1 = 4.4
            DiscountApplicationOnPricePreTaxIsCorrectForIndividuallyPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4.4);
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
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountPercentage(10).Build();

            //check for various quantities - should be fine with individual item pricing to just multiply up
            for (int quantity = 1; quantity <= 10; quantity++)
            {
                cartItemData.Quantity = quantity;
                pricingMethod.GetPricePreTax(cartItemData).Should().Be(Convert.ToDecimal(expected * quantity));
            }
        }
    }
}