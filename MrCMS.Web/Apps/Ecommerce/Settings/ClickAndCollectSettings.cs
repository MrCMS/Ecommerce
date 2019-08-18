using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class ClickAndCollectSettings : SiteSettingsBase
    {
        public ClickAndCollectSettings()
        {
            DisplayName = "Click and Collect";
        }

        [Required]
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}