using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers.Pricing;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface ISetTaxDetails
    {
        void SetOrderLinesTaxes(ref Order order);
        void SetShippingTaxes(ref Order order);
    }
    public class SetTaxDetails : ISetTaxDetails
    {
        private readonly AmazonSyncSettings _amazonSyncSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IProductPricingMethod _productPricingMethod;
        private readonly IProductVariantService _productVariantService;
        private readonly IGetProductVariantTaxRatePercentage _getProductVariantTaxRatePercentage;

        public SetTaxDetails(AmazonSyncSettings amazonSyncSettings, TaxSettings taxSettings, 
            ITaxRateManager taxRateManager, IProductPricingMethod productPricingMethod, IProductVariantService productVariantService, IGetProductVariantTaxRatePercentage getProductVariantTaxRatePercentage)
        {
            _amazonSyncSettings = amazonSyncSettings;
            _taxSettings = taxSettings;
            _taxRateManager = taxRateManager;
            _productPricingMethod = productPricingMethod;
            _productVariantService = productVariantService;
            _getProductVariantTaxRatePercentage = getProductVariantTaxRatePercentage;
        }

        public void SetOrderLinesTaxes(ref Order order)
        {
            foreach (var orderLine in order.OrderLines)
            {
                var productVariant = _productVariantService.GetProductVariantBySKU(orderLine.SKU);
                if (productVariant == null)
                    continue;

                var tax = _productPricingMethod.GetUnitTax(productVariant, 0m, 0m);

                                                                       //new TaxSettings
                                                                       //    {
                                                                       //        TaxesEnabled = true,
                                                                       //        LoadedPricesIncludeTax =true
                                                                       //    });
                //var tax = taxAwareProductPrice.Tax.GetValueOrDefault();
                orderLine.UnitPricePreTax = orderLine.UnitPrice - tax;
                orderLine.PricePreTax = orderLine.UnitPricePreTax*orderLine.Quantity;
                orderLine.Tax = orderLine.Price - orderLine.PricePreTax;
                orderLine.TaxRate = _getProductVariantTaxRatePercentage.GetTaxRatePercentage(productVariant);

            }
            order.Subtotal = order.OrderLines.Sum(line => line.UnitPricePreTax*line.Quantity);
            order.Tax = order.OrderLines.Sum(line => line.Tax);
        }

        public void SetShippingTaxes(ref Order order)
        {
            if (!_taxSettings.ShippingRateTaxesEnabled || !order.ShippingTotal.HasValue || !_amazonSyncSettings.UseDefaultTaxRateForShippingTax) return;

            var taxRate = _taxRateManager.GetDefaultRate();

            if (taxRate == null) return;
            //var taxAwareProductPrice = ,
            //                                                           new TaxSettings
            //                                                           {
            //                                                               TaxesEnabled = true,
            //                                                               ShippingRateIncludesTax = true,
            //                                                               ShippingRateTaxesEnabled = true
            //                                                           });
            order.ShippingTax = order.ShippingTotal.ShippingTax(taxRate.Percentage);
            order.ShippingTaxPercentage = taxRate.Percentage;
            order.Tax += order.ShippingTax.GetValueOrDefault();
        }
    }
}