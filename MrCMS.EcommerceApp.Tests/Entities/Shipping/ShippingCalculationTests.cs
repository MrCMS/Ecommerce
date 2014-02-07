using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Ninject.MockingKernel;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public abstract class ShippingCalculationTests : MrCMSTest
    {
        private readonly TaxSettings _taxSettings;

        protected ShippingCalculationTests()
        {
            _taxSettings = new TaxSettings();
            Kernel.Bind<TaxSettings>().ToConstant(TaxSettings);
        }

        protected TaxSettings TaxSettings
        {
            get { return _taxSettings; }
        }
    }

    [DoNotMap]
    public class TestableShippingCalculation : ShippingCalculation
    {
        private readonly bool? _canBeUsed;
        private readonly decimal? _price;
        private readonly decimal? _tax;
        private readonly TaxRate _taxRate;
        private IList<ProductVariant> _productVariants;

        public TestableShippingCalculation(bool? canBeUsed = null, decimal? price = null, decimal taxRate = 0m, decimal? tax = null)
        {
            _canBeUsed = canBeUsed;
            _price = price;
            _tax = tax;
            _taxRate = new TaxRate { Percentage = taxRate };
        }

        public override bool CanBeUsed(CartModel model)
        {
            return _canBeUsed ?? base.CanBeUsed(model);
        }

        public override TaxRate TaxRate
        {
            get { return _taxRate; }
        }

        public override decimal Tax
        {
            get { return _tax ?? base.Tax; }
        }

        public override decimal? GetPrice(CartModel model)
        {
            return _price ?? base.GetPrice(model);
        }

        public void SetExcludedProductVariants(IList<ProductVariant> productVariants)
        {
            _productVariants = productVariants;
        }

        public override IList<ProductVariant> ExcludedProductVariants
        {
            get { return _productVariants ?? base.ExcludedProductVariants; }
        }
    }
}