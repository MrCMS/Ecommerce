using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings.Shipping
{
    public class UKFirstClassShippingSettings : SiteSettingsBase
    {
        public UKFirstClassShippingSettings()
        {
            DisplayName = "UK First Class";
        }

        [Required]
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        [DisplayName("Tax Rate")]
        public int? TaxRateId { get; set; }
    }
}