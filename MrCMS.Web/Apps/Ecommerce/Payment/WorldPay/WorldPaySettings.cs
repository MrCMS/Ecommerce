using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.WorldPay
{
    public class WorldPaySettings : SiteSettingsBase
    {
        [DisplayName("Requires SSL")]
        public bool RequiresSSL { get; set; }

        public bool Enabled { get; set; }

        [DisplayName("Use Sandbox")]
        public bool UseSandbox { get; set; }
        [DisplayName("Instance Id")]
        public string InstanceId { get; set; }
        [DisplayName("Payment Method")]
        public string PaymentMethod { get; set; }
        [DisplayName("Callback Password")]
        public string CallbackPassword { get; set; }
        [DisplayName("CSS")]
        public string CssName { get; set; }

        public string GetPostUrl()
        {
            return UseSandbox
                ? "https://secure-test.wp3.rbsworldpay.com/wcc/purchase"
                : "https://secure.wp3.rbsworldpay.com/wcc/purchase";
        }
    }
}