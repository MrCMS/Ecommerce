using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class GiftCardSettings : SiteSettingsBase
    {
        [DisplayName("Activate On")]
        public ActivateOn ActivateOn { get; set; }
    }

    public enum ActivateOn
    {
        Payment,
        Shipping,
        OrderCompletion
    }
}