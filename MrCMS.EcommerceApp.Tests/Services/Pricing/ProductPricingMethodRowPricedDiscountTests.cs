using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.Pricing
{
    public class ProductPricingMethodRowPricedDiscountSingleItemTests
    {
        private const decimal TaxPercentage = 20;

        [Fact]
        public void PriceTest1()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.8);
        }

        [Fact]
        public void PriceTest2()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 5);
        }

        [Fact]
        public void PriceTest3()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.8);
        }

        [Fact]
        public void PriceTest4()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5);
        }

        [Fact]
        public void PriceTest5()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.8);
        }

        [Fact]
        public void PriceTest6()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 5);
        }

        [Fact]
        public void PriceTest7()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.8);
        }

        [Fact]
        public void PriceTest8()
        {
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5);
        }

        private void DiscountApplicationOnPriceIsCorrectForSingleRowPriced(double basePrice,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder()
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .CalculateTaxOnRow()
                    .WithDiscountOnPrices(discountOnPrices)
                    .WithCustomerTaxApplication(applyCustomerTax)
                    .Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountAmount(1).Build();

            pricingMethod.GetPrice(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Fact]
        public void TaxTest1()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 0.8);
        }

        [Fact]
        public void TaxTest2()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 0.83);
        }

        [Fact]
        public void TaxTest3()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        [Fact]
        public void TaxTest4()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        [Fact]
        public void TaxTest5()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 0.8);
        }

        [Fact]
        public void TaxTest6()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 0.83);
        }

        [Fact]
        public void TaxTest7()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        [Fact]
        public void TaxTest8()
        {
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 1);
        }

        private void DiscountApplicationOnTaxIsCorrectForSingleRowPriced(double basePrice,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder()
                    .WithPriceLoadingMethod(priceLoadingMethod)
                    .CalculateTaxOnRow()
                    .WithDiscountOnPrices(discountOnPrices)
                    .WithCustomerTaxApplication(applyCustomerTax)
                    .Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(Convert.ToDecimal(basePrice))
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountAmount(1).Build();

            pricingMethod.GetTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Fact]
        public void PricePreTaxTest1()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4);
        }

        [Fact]
        public void PricePreTaxTest2()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.17);
        }

        [Fact]
        public void PricePreTaxTest3()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 3.8);
        }

        [Fact]
        public void PricePreTaxTest4()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4);
        }

        [Fact]
        public void PricePreTaxTest5()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4);
        }

        [Fact]
        public void PricePreTaxTest6()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.17);
        }

        [Fact]
        public void PricePreTaxTest7()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 3.8);
        }

        [Fact]
        public void PricePreTaxTest8()
        {
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(6, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4);
        }

        private void DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(decimal basePrice,
            PriceLoadingMethod priceLoadingMethod, DiscountOnPrices discountOnPrices,
            ApplyCustomerTax applyCustomerTax, double expected)
        {
            var pricingMethod =
                new ProductPricingMethodBuilder().WithPriceLoadingMethod(priceLoadingMethod)
                    .CalculateTaxOnRow()
                    .WithDiscountOnPrices(discountOnPrices)
                    .WithCustomerTaxApplication(applyCustomerTax)
                    .Build();
            var productVariant =
                new ProductVariantBuilder().WithBasePrice(basePrice)
                    .WithTaxPercentage(TaxPercentage)
                    .Build();
            var cartItemData = new CartItemBuilder().WithItem(productVariant).WithDiscountAmount(1).Build();

            pricingMethod.GetPricePreTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }
    }
}