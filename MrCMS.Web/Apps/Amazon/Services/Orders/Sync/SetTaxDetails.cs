using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;

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
            decimal totalTax = 0;
            order.Subtotal = 0;
            foreach (var orderLine in order.OrderLines)
            {
                var taxRate = _taxRateManager.GetDefaultRate(orderLine);

                if (taxRate == null) continue;

                orderLine.UnitPricePreTax = orderLine.UnitPrice-taxRate.GetTaxForAmount(orderLine.UnitPrice);
                order.Subtotal += orderLine.UnitPricePreTax*orderLine.Quantity;
                orderLine.PricePreTax = orderLine.Price - taxRate.GetTaxForAmount(orderLine.Price);
                orderLine.Tax = taxRate.GetTaxForAmount(orderLine.Price);
                orderLine.TaxRate = taxRate.Percentage;

                totalTax += orderLine.Tax;
            }
            order.Tax = totalTax;
        }

        public void SetShippingTaxes(ref Order order)
        {
            if (!_taxSettings.ShippingRateTaxesEnabled || !order.ShippingTotal.HasValue || !_amazonSyncSettings.UseDefaultTaxRateForShippingTax) return;

            var taxRate = _taxRateManager.GetDefaultRate();

            if (taxRate == null) return;

            order.ShippingTax = taxRate.GetTaxForAmount(order.ShippingTotal.Value);
            order.ShippingTaxPercentage = taxRate.Percentage;
        }
    }
}