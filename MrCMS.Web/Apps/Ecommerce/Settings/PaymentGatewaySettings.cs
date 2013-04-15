using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class PaymentGatewaySettings : SiteSettingsBase
    {
        [DisplayName("Uses Paypal Express Checkout")]
        public bool UsesPaypalExpressCheckout { get; set; }

        [DisplayName("Paypal Express Checkout Url")]
        public string PaypalExpressCheckoutUrl { get; set; }

        [DisplayName("Uses Google Checkout")]
        public bool UsesGoogleCheckout { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }
    }
}