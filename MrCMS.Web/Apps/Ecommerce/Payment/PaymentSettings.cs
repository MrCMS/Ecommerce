using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment
{
    public class PaymentSettings : SiteSettingsBase
    {
        [DisplayName("Cash on delivery enabled")]
        public bool CashOnDeliveryEnabled { get; set; }
    }
}