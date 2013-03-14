using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class StoreSettings : SiteSettingsBase
    {
        [DisplayName("Loaded Prices Include Tax")]
        public bool LoadedPricesIncludeTax { get; set; }
    }
}