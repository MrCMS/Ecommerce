using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings.Shipping
{
    public class UKFirstClassShippingSettings : SiteSettingsBase
    {
        public UKFirstClassShippingSettings()
        {
            Name = "UK First Class";
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? TaxRateId { get; set; }
    }
}