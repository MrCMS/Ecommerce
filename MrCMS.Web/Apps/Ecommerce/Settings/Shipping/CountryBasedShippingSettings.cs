using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings.Shipping
{
    public class CountryBasedShippingSettings : SiteSettingsBase
    {
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Tax Rate")]
        public int? TaxRateId { get; set; }
    }
}