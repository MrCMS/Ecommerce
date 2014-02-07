using System.Collections.Generic;
using System.Linq;
using MrCMS.EcommerceApp.Tests.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ShippingCalculationBuilder
    {
        private decimal? _baseAmount;
        private bool? _canBeUsed;
        private Country _country;
        private ShippingCriteria _criteria = ShippingCriteria.ByCartTotal;
        private decimal _lowerBound;
        private decimal? _price;
        private decimal? _tax;
        private decimal _taxRate;
        private decimal? _upperBound;
        private bool _methodCanBeUsed = true;
        private readonly IList<ProductVariant> _excludedProductVariants = new List<ProductVariant>();

        public ShippingCalculationBuilder WithCanBeUsed(bool canBeUsed)
        {
            _canBeUsed = canBeUsed;
            return this;
        }

        public ShippingCalculationBuilder WithPrice(decimal price)
        {
            _price = price;
            return this;
        }

        public ShippingCalculationBuilder WithBaseAmount(decimal baseAmount)
        {
            _baseAmount = baseAmount;
            return this;
        }

        public ShippingCalculationBuilder WithTaxRate(decimal taxRate)
        {
            _taxRate = taxRate;
            return this;
        }

        public ShippingCalculationBuilder WithShippingCriteria(ShippingCriteria criteria)
        {
            _criteria = criteria;
            return this;
        }

        public ShippingCalculationBuilder WithLowerBound(decimal lowerBound)
        {
            _lowerBound = lowerBound;
            return this;
        }

        public ShippingCalculationBuilder WithUpperBound(decimal? upperBound)
        {
            _upperBound = upperBound;
            return this;
        }

        public ShippingCalculationBuilder WithTax(decimal tax)
        {
            _tax = tax;
            return this;
        }

        public ShippingCalculationBuilder WithCountry(Country country)
        {
            _country = country;
            return this;
        }

        public ShippingCalculationBuilder WithRestrictedVariant(ProductVariant productVariant)
        {
            _excludedProductVariants.Add(productVariant);
            return this;
        }

        public ShippingCalculation Build()
        {
            var calculation = new TestableShippingCalculation(_canBeUsed, _price, _taxRate, _tax)
                                  {
                                      ShippingCriteria = _criteria,
                                      LowerBound = _lowerBound,
                                      UpperBound = _upperBound,
                                      Country = _country,
                                      ShippingMethod = new TestableShippingMethod(_methodCanBeUsed)
                                  };
            if (_baseAmount.HasValue)
                calculation.BaseAmount = _baseAmount.Value;
            if (_excludedProductVariants.Any())
                calculation.SetExcludedProductVariants(_excludedProductVariants);

            return calculation;
        }
    }
}