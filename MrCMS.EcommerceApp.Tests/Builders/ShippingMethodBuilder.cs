using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.EcommerceApp.Tests.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ShippingMethodBuilder
    {
        private IList<ShippingCalculation> _shippingCalculations = new List<ShippingCalculation>
                                                                       {
                                                                           new TestableShippingCalculation(canBeUsed: true)
                                                                       };

        private readonly IList<ProductVariant> _excludedProductVariants = new List<ProductVariant>();

        private bool? _canBeUsed;

        public ShippingMethodBuilder WithOnlyTheCalculations(params ShippingCalculation[] calculations)
        {
            _shippingCalculations = new List<ShippingCalculation>(calculations);
            return this;
        }

        public ShippingMethodBuilder WithCanBeUsed(bool canBeUsed)
        {
            _canBeUsed = canBeUsed;
            return this;
        }

        public ShippingMethod Build()
        {
            return new TestableShippingMethod(canBeUsed: _canBeUsed)
                       {
                           ShippingCalculations = _shippingCalculations,
                           ExcludedProductVariants = _excludedProductVariants
                       };
        }
    }

    [DoNotMap]
    public class TestableShippingMethod : ShippingMethod
    {
        private readonly bool? _canBeUsed;

        public TestableShippingMethod(bool? canBeUsed = null)
        {
            _canBeUsed = canBeUsed;
        }
        public override bool CanBeUsed(CartModel model)
        {
            return _canBeUsed ?? base.CanBeUsed(model);
        }
    }
}