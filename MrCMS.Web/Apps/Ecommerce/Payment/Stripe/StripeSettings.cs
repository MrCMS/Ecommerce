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

        [DisplayName("Enabled?")]
        public bool Enabled { get; set; }
        [DisplayName("Use Sandbox?")]
        public bool UseSandbox { get; set; }

        [DisplayName("Public Key")]
        public string PublicKey { get; set; }

        [DisplayName("Private Key")]
        public string PrivateKey { get; set; }
        public override bool RenderInSettings => false;

        [DisplayName("3D Secure Required?")]
        public bool ThreeDSecureRequired { get; set; }
    }
}