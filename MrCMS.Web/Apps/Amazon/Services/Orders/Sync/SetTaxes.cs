using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface ISetTaxes
    {
        void SetTax(ref Order order,decimal tax);
    }
    public class SetTaxes : ISetTaxes
    {
        private readonly AmazonSyncSettings _amazonSyncSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ISetTaxDetails _setTaxDetails;

        public SetTaxes(AmazonSyncSettings amazonSyncSettings, TaxSettings taxSettings,
            ISetTaxDetails taxRateManager)
        {
            _amazonSyncSettings = amazonSyncSettings;
            _taxSettings = taxSettings;
            _setTaxDetails = taxRateManager;
        }

        public void SetTax(ref Order order, decimal tax)
        {
            if (tax > 0)
            {
                order.Tax = tax;
                return;
            }

            if (!_amazonSyncSettings.TryCalculateVat || !_taxSettings.TaxesEnabled) return;

            _setTaxDetails.SetOrderLinesTaxes(ref order);

            _setTaxDetails.SetShippingTaxes(ref order);
        }
    }
}