using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.Pricing
{
    public class ProductPricingMethodRowPricedDiscountMultipleItemTests
    {
        private const decimal TaxPercentage = 20;

        [Fact]
        public void PriceTest1()
        {
            // ((5.03 - 1) * 1.2) * 5 = 24.18
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 24.18);
        }

        [Fact]
        public void PriceTest2()
        {
            // ((5.03) * 1.2) - 1 * 5 = 25.18
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 25.18);
        }

        [Fact]
        public void PriceTest3()
        {
            // tax calculation shouldn't affect bottom line, so same as #1
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 24.18);
        }

        [Fact]
        public void PriceTest4()
        {
            // tax calculation shouldn't affect bottom line, so same as #2
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 25.18);
        }

        [Fact]
        public void PriceTest5()
        {

            // ((5.03 - 1.2)) * 5 = 19.15 - 1.2 is the discount applied to 5.03/1.2 (20% tax) then multiplied back up
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 19.15);
        }

        [Fact]
        public void PriceTest6()
        {
            // ((5.03 - 1)) * 5 = 20.15
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 20.15);
        }

        [Fact]
        public void PriceTest7()
        {
            // tax calculation shouldn't affect bottom line, so same as #5
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 19.15);
        }

        [Fact]
        public void PriceTest8()
        {
            // tax calculation shouldn't affect bottom line, so same as #6
            DiscountApplicationOnPriceIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 20.15);
        }

        private void DiscountApplicationOnPriceIsCorrectForSingleRowPriced(double basePrice, int quantity,
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
            var cartItemData =
                new CartItemBuilder().WithQuantity(quantity).WithItem(productVariant).WithDiscountAmount(1).Build();

            pricingMethod.GetPrice(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Fact]
        public void TaxTest1()
        {
            // ((5.03 - 1) * 0.2) * 5 = 4.03
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 4.03);
        }

        [Fact]
        public void TaxTest2()
        {
            // ((5.03 * 1.2) - 1) * 5 = 25.18
            // tax on 25.18 = 25.18 / 1.2 * 0.2 = 4.19666.. ~= 4.20
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 4.20);
        }

        [Fact]
        public void TaxTest3()
        {
            // tax is calculated as if without discount so:
            // 5.03 * 1.2 * 5 = 30.18
            // tax on 30.18 = 30.18 / 1.2 * 0.2 = 5.03
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 5.03);
        }

        [Fact]
        public void TaxTest4()
        {
            // tax is calculated as if without discount so:
            // 5.03 * 1.2 * 5 = 30.18
            // tax on 30.18 = 30.18 / 1.2 * 0.2 = 5.03
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 5.03);
        }

        [Fact]
        public void TaxTest5()
        {
            // total as per price test 5 = ((5.03 - 1.2)) * 5 = 19.15
            // then as it's after discount get tax on 19.15 so
            //  tax on 19.15 = 19.15 / 1.2 * 0.2 = 3.191111... ~= 3.19
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 3.19);
        }

        [Fact]
        public void TaxTest6()
        {
            // ((5.03 - 1)) * 5 = 20.15
            // total as per price test 6 = ((5.03 - 1)) * 5 = 20.15
            // then as it's after discount get tax on 20.15 so
            //  tax on 20.15 = 20.15 / 1.2 * 0.2 = 3.358333... ~= 3.36
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 3.36);
        }

        [Fact]
        public void TaxTest7()
        {
            // tax is calculated as if without discount so:
            // 5.03  * 5 = 25.15
            // tax on 25.15 = 25.15 / 1.2 * 0.2 = 4.191666.. ~= 4.19
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 4.19);
        }

        [Fact]
        public void TaxTest8()
        {
            // tax is calculated as if without discount so:
            // 5.03  * 5 = 25.15
            // tax on 25.15 = 25.15 / 1.2 * 0.2 = 4.191666.. ~= 4.19
            DiscountApplicationOnTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 4.19);
        }

        private void DiscountApplicationOnTaxIsCorrectForSingleRowPriced(double basePrice, int quantity,
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
            var cartItemData =
                new CartItemBuilder().WithQuantity(quantity).WithItem(productVariant).WithDiscountAmount(1).Build();

            pricingMethod.GetTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }

        [Fact]
        public void PricePreTaxTest1()
        {
            // Price for 1 - Tax for 1 = 24.18 - 4.03
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 20.15);
        }

        [Fact]
        public void PricePreTaxTest2()
        {
            // Price for 2 - Tax for 2 = 25.18 - 4.203
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 20.98);
        }

        [Fact]
        public void PricePreTaxTest3()
        {
            // Price for 3 - Tax for 3 = 24.18 - 5.03
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 19.15);
        }

        [Fact]
        public void PricePreTaxTest4()
        {
            // Price for 4 - Tax for 4 = 25.18 - 5.03
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.ExcludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 20.15);
        }

        [Fact]
        public void PricePreTaxTest5()
        {
            // Price for 5 - Tax for 5 = 19.15 - 3.19
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.AfterDiscount, 15.96);
        }

        [Fact]
        public void PricePreTaxTest6()
        {
            // Price for 6 - Tax for 6 = 20.15 - 3.36
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.AfterDiscount, 16.79);
        }

        [Fact]
        public void PricePreTaxTest7()
        {
            // Price for 7 - Tax for 7 = 19.15 - 4.19
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.ExcludingTax, ApplyCustomerTax.BeforeDiscount, 14.96);
        }

        [Fact]
        public void PricePreTaxTest8()
        {
            // Price for 8 - Tax for 8 = 20.15 - 4.19
            DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(5.03, 5, PriceLoadingMethod.IncludingTax,
                DiscountOnPrices.IncludingTax, ApplyCustomerTax.BeforeDiscount, 15.96);
        }

        private void DiscountApplicationOnPricePreTaxIsCorrectForSingleRowPriced(double basePrice, int quantity,
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
            var cartItemData =
                new CartItemBuilder().WithQuantity(quantity).WithItem(productVariant).WithDiscountAmount(1).Build(); ;

            pricingMethod.GetPricePreTax(cartItemData).Should().Be(Convert.ToDecimal(expected));
        }
    }
}