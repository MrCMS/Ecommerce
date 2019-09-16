using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe
{
    public class StripeSettings : SiteSettingsBase
    {
        public StripeSettings()
        {
            ThreeDSecureRequired = true;
        }

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


        [DisplayName("3D Secure Required?")]
        public bool ThreeDSecureRequired { get; set; }

        public string MerchantId { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}