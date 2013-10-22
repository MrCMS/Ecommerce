using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface ISetTax
    {
        void SetTaxes(ref Order order,decimal tax);
    }
    public class SetTax : ISetTax
    {
        private readonly AmazonSyncSettings _amazonSyncSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IProductVariantService _productVariantService;

        public SetTax(AmazonSyncSettings amazonSyncSettings, TaxSettings taxSettings, 
            ITaxRateManager taxRateManager, IProductVariantService productVariantService)
        {
            _amazonSyncSettings = amazonSyncSettings;
            _taxSettings = taxSettings;
            _taxRateManager = taxRateManager;
            _productVariantService = productVariantService;
        }

        public void SetTaxes(ref Order order, decimal tax)
        {
            if (tax > 0)
                order.Tax=tax;

            if (!_amazonSyncSettings.TryCalculateVat || !_taxSettings.TaxesEnabled) return;

            SetTaxDetails(ref order);
        }

        private void SetTaxDetails(ref Order order)
        {
            var totalTax = SetOrderLinesTaxes(ref order);
            order.Tax = totalTax;
            order.Total += totalTax;

            SetShippingTaxes(ref order);
        }

        private decimal SetOrderLinesTaxes(ref Order order)
        {
            decimal totalTax = 0;
            foreach (var orderLine in order.OrderLines)
            {
                TaxRate taxRate = null;
                var pv = _productVariantService.GetProductVariantBySKU(orderLine.SKU);
                if (pv != null && pv.TaxRate != null)
                    taxRate = pv.TaxRate;
                if(taxRate == null)
                    taxRate = _taxRateManager.GetDefaultRate();

                if (taxRate == null) continue;

                orderLine.UnitPricePreTax = orderLine.UnitPrice;
                orderLine.UnitPrice += taxRate.GetTaxForAmount(orderLine.UnitPrice);
                orderLine.PricePreTax = orderLine.Price;
                orderLine.Price += taxRate.GetTaxForAmount(orderLine.Price);
                orderLine.Tax = taxRate.GetTaxForAmount(orderLine.Price);
                orderLine.TaxRate = taxRate.Percentage;
                totalTax += orderLine.Tax;
            }
            return totalTax;
        }

        private void SetShippingTaxes(ref Order order)
        {
            if (!_taxSettings.ShippingRateTaxesEnabled || !order.ShippingTotal.HasValue || !_amazonSyncSettings.UseDefaultTaxRateForShippingTax) return;

            var taxRate = _taxRateManager.GetDefaultRate();
            order.ShippingTax = taxRate.GetTaxForAmount(order.ShippingTotal.Value);
            order.ShippingTaxPercentage = taxRate.Percentage;
            order.Total += order.ShippingTax.Value;
        }
    }
}