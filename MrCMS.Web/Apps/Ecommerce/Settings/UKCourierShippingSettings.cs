using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class UKCourierShippingSettings : SiteSettingsBase
    {
        public UKCourierShippingSettings()
        {
            DisplayName = "Courier Delivery";
        }

        [Required]
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        [DisplayName("Tax Rate")]
        public int? TaxRateId { get; set; }
    }
}