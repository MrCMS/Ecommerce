using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ProductPricingMethodBuilder
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate = A.Fake<IGetDefaultTaxRate>();
        private TaxCalculationMethod _taxCalculationMethod;
        private PriceLoadingMethod _priceLoadingMethod;
        private bool _taxesEnabled = true;
        private ApplyCustomerTax _applyCustomerTax;
        private DiscountOnPrices _discountOnPrices;

        public ProductPricingMethodBuilder LoadedPricesIncludeTax()
        {
            _priceLoadingMethod = PriceLoadingMethod.IncludingTax;
            return this;
        }

        public ProductPricingMethodBuilder WithPriceLoadingMethod(PriceLoadingMethod priceLoadingMethod)
        {
            _priceLoadingMethod = priceLoadingMethod;
            return this;
        }

        public ProductPricingMethodBuilder LoadedPricesExcludeTax()
        {
            _priceLoadingMethod = PriceLoadingMethod.ExcludingTax;
            return this;
        }

        public ProductPricingMethodBuilder CalculateTaxOnIndividualItem()
        {
            _taxCalculationMethod = TaxCalculationMethod.Individual;
            return this;
        }

        public ProductPricingMethodBuilder CalculateTaxOnRow()
        {
            _taxCalculationMethod = TaxCalculationMethod.Row;
            return this;
        }

        public ProductPricingMethodBuilder WithTaxCalculationMethod(TaxCalculationMethod taxCalculationMethod)
        {
            _taxCalculationMethod = taxCalculationMethod;
            return this;
        }

        public ProductPricingMethodBuilder TaxesDisabled()
        {
            _taxesEnabled = false;
            return this;
        }


        public ProductPricingMethodBuilder WithDefaultTaxRate(decimal percentage)
        {
            A.CallTo(() => _getDefaultTaxRate.Get()).Returns(new TaxRate {Percentage = percentage});
            return this;
        }

        public ProductPricingMethod Build()
        {
            return new ProductPricingMethod(new TaxSettings
            {
                PriceLoadingMethod = _priceLoadingMethod,
                TaxCalculationMethod = _taxCalculationMethod,
                TaxesEnabled = _taxesEnabled,
                ApplyCustomerTax = _applyCustomerTax,
                DiscountOnPrices = _discountOnPrices
            }, _getDefaultTaxRate);
        }

        public ProductPricingMethodBuilder TaxesEnabled(bool taxesEnabled)
        {
            _taxesEnabled = taxesEnabled;
            return this;
        }

        public ProductPricingMethodBuilder WithDiscountOnPrices(DiscountOnPrices discountOnPrices)
        {
            _discountOnPrices = discountOnPrices;
            return this;
        }

        public ProductPricingMethodBuilder WithCustomerTaxApplication(ApplyCustomerTax applyCustomerTax)
        {
            _applyCustomerTax = applyCustomerTax;
            return this;
        }
    }
}