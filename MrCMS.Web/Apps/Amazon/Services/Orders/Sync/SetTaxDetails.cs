using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using System.Linq;

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

        public SetTaxDetails(AmazonSyncSettings amazonSyncSettings, TaxSettings taxSettings, 
            ITaxRateManager taxRateManager)
        {
            _amazonSyncSettings = amazonSyncSettings;
            _taxSettings = taxSettings;
            _taxRateManager = taxRateManager;
        }

        public void SetOrderLinesTaxes(ref Order order)
        {
            foreach (var orderLine in order.OrderLines)
            {
                var taxRate = _taxRateManager.GetRateForOrderLine(orderLine);

                if (taxRate == null) continue;

                var taxAwareProductPrice = TaxAwareProductPrice.Create(orderLine.UnitPrice, taxRate,
                                                                       new TaxSettings
                                                                           {
                                                                               TaxesEnabled = true,
                                                                               LoadedPricesIncludeTax =true
                                                                           });
                var tax = taxAwareProductPrice.Tax.GetValueOrDefault();
                orderLine.UnitPricePreTax = orderLine.UnitPrice - tax;
                orderLine.PricePreTax = orderLine.UnitPricePreTax*orderLine.Quantity;
                orderLine.Tax = orderLine.Price - orderLine.PricePreTax;
                orderLine.TaxRate = taxRate.Percentage;

            }
            order.Subtotal = order.OrderLines.Sum(line => line.UnitPricePreTax*line.Quantity);
            order.Tax = order.OrderLines.Sum(line => line.Tax);
        }

        public void SetShippingTaxes(ref Order order)
        {
            if (!_taxSettings.ShippingRateTaxesEnabled || !order.ShippingTotal.HasValue || !_amazonSyncSettings.UseDefaultTaxRateForShippingTax) return;

            var taxRate = _taxRateManager.GetDefaultRate();

            if (taxRate == null) return;
            var taxAwareProductPrice = TaxAwareShippingRate.Create(order.ShippingTotal, taxRate,
                                                                       new TaxSettings
                                                                       {
                                                                           TaxesEnabled = true,
                                                                           ShippingRateIncludesTax = true,
                                                                           ShippingRateTaxesEnabled = true
                                                                       });
            order.ShippingTax = taxAwareProductPrice.Tax.GetValueOrDefault();
            order.ShippingTaxPercentage = taxRate.Percentage;
            order.Tax += order.ShippingTax.GetValueOrDefault();
        }
    }
}