using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon
{
    public class ElavonSettings : SiteSettingsBase
    {
        public ElavonSettings()
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


        [DisplayName("3D Secure Required? ha")]
        public bool ThreeDSecureRequired { get; set; }
        public string SharedSecret { get; set; }

        public string MerchantId { get; set; }

        public string AccountId { get; set; }

        // Elavon Service Url, which is either sandbox or live environment endpoint
        // e.g. "https://pay.sandbox.elavonpaymentgateway.com/pay" for testing sandbox
        public string ServiceUrl { get; set; }  

    }
}