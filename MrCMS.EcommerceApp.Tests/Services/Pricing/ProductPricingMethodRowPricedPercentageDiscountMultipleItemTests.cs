using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.Pricing
{
    public class ProductPricingMethodRowPricedPercentageDiscountMultipleItemTests
    {
        private const decimal TaxPercentage = 20;

        [Fact]
        public void PriceTest1()
        {
            // (5.03 * 1.2 * 5) - 10% = 27.162 ~= 27.16
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 27.16);
        }

        [Fact]
        public void PriceTest2()
        {
            // (5.03 * 1.2 * 5) - 10% = 27.162 ~= 27.16
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 27.16);
        }

        [Fact]
        public void PriceTest3()
        {
            // (5.03 * 1.2 * 5) - 10% = 27.162 ~= 27.16
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 27.16);
        }

        [Fact]
        public void PriceTest4()
        {
            // (5.03 * 1.2 * 5) - 10% = 27.162 ~= 27.16
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 27.16);
        }

        [Fact]
        public void PriceTest5()
        {
            // (5.03 * 5) - 10% = 25.15 - 2.52 = 22.63
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 22.63);
        }

        [Fact]
        public void PriceTest6()
        {
            // (5.03 * 5) - 10% = 25.15 - 2.52 = 22.63
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 22.63);
        }

        [Fact]
        public void PriceTest7()
        {
            // (5.03 * 5) - 10% = 25.15 - 2.52 = 22.63
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 22.63);
        }

        [Fact]
        public void PriceTest8()
        {
            // (5.03 * 5) - 10% = 25.15 - 2.52 = 22.63
            DiscountApplicationOnPriceIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 22.63);
        }

        private void DiscountApplicationOnPriceIsCorrectForRowPriced(decimal basePrice, int quantity,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                GetPricingMethod(priceLoadingMethod, discountOnPrices, applyCustomerTax);
            var cartItemData = GetCartItemData(basePrice, quantity);

            pricingMethod.GetPrice(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Fact]
        public void TaxTest1()
        {
            // Tax: 5.03 - 10% = 4.527 * 0.2 for 20% tax = 0.9054 * 5 for quantity = 4.527 ~= 4.53
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.53);
        }

        [Fact]
        public void TaxTest2()
        {
            // Tax: 5.03 - 10% = 4.527 * 0.2 for 20% tax = 0.9054 * 5 for quantity = 4.527 ~= 4.53
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.53);
        }

        [Fact]
        public void TaxTest3()
        {
            // Tax is calculated as if they were on the full amount
            // Tax: 5.03 * 0.2 for 20% tax = 1.006 * 5 for quantity = 5.03
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 5.03);
        }

        [Fact]
        public void TaxTest4()
        {
            // Tax is calculated as if they were on the full amount
            // Tax: 5.03 * 0.2 for 20% tax = 1.006 * 5 for quantity = 5.03
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5.03);
        }

        [Fact]
        public void TaxTest5()
        {
            // Tax: 5.03 - 10% = 4.527 * 0.2/1.2 for 20% tax = 0.7545 * 5 for quantity = 3.7725 ~= 3.77
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 3.77);
        }

        [Fact]
        public void TaxTest6()
        {
            // Tax: 5.03 - 10% = 4.527 * 0.2/1.2 for 20% tax = 0.7545 * 5 for quantity = 3.7725 ~= 3.77
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 3.77);
        }

        [Fact]
        public void TaxTest7()
        {
            // Tax is calculated as if they were on the full amount
            // Tax: 5.03 * 0.2/1.2 for 20% tax = 0.838333... * 5 for quantity = 4.191666... ~= 4.19
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.19);
        }

        [Fact]
        public void TaxTest8()
        {
            // Tax is calculated as if they were on the full amount
            // Tax: 5.03 * 0.2/1.2 for 20% tax = 0.838333... * 5 for quantity = 4.191666... ~= 4.19
            DiscountApplicationOnTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4.19);
        }

        private void DiscountApplicationOnTaxIsCorrectForRowPriced(decimal basePrice, int quantity,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                GetPricingMethod(priceLoadingMethod, discountOnPrices, applyCustomerTax);
            var cartItemData = GetCartItemData(basePrice, quantity);

            pricingMethod.GetTax(cartItemData).Should().Be(Convert.ToDecimal(expected ));
        }

        [Fact]
        public void PricePreTaxTest1()
        {
            // 27.16 - 4.53
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 22.63);
        }

        [Fact]
        public void PricePreTaxTest2()
        {
            // 27.16 - 4.53
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 22.63);
        }

        [Fact]
        public void PricePreTaxTest3()
        {
            // 27.16 - 5.03
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 22.13);
        }

        [Fact]
        public void PricePreTaxTest4()
        {
            // 27.16 - 5.03
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 22.13);
        }

        [Fact]
        public void PricePreTaxTest5()
        {
            // 22.63 - 3.77
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 18.86);
        }

        [Fact]
        public void PricePreTaxTest6()
        {
            // 22.63 - 3.77
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 18.86);
        }

        [Fact]
        public void PricePreTaxTest7()
        {
            // 22.63 - 4.19
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 18.44);
        }

        [Fact]
        public void PricePreTaxTest8()
        {
            // 22.63 - 4.194
            DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(5.03m, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 18.44);
        }

        private void DiscountApplicationOnPricePreTaxIsCorrectForRowPriced(decimal basePrice, int quantity,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                GetPricingMethod(priceLoadingMethod, discountOnPrices, applyCustomerTax);
            var cartItemData = GetCartItemData(basePrice, quantity);

            pricingMethod.GetPricePreTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        private static CartItemData GetCartItemData(decimal basePrice, int quantity)
        {
            var productVariant = new ProductVariantBuilder()
                .WithBasePrice(basePrice)
                .WithTaxPercentage(TaxPercentage)
                .Build();

            return new CartItemBuilder()
                .WithQuantity(quantity)
                .WithItem(productVariant)
                .WithDiscountPercentage(10)
                .Build();
        }

        private static ProductPricingMethod GetPricingMethod(PriceLoadingMethod priceLoadingMethod,
            DiscountOnPrices discountOnPrices, ApplyCustomerTax applyCustomerTax)
        {
            return new ProductPricingMethodBuilder().WithPriceLoadingMethod(priceLoadingMethod)
                .CalculateTaxOnRow()
                .WithDiscountOnPrices(discountOnPrices)
                .WithCustomerTaxApplication(applyCustomerTax)
                .Build();
        }
    }
}