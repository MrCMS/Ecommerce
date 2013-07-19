using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalExpressCheckoutSettings : SettingsBase
    {
        [DisplayName("Is Live?")]
        public bool IsLive { get; set; }

        [DisplayName("Locale Code")]
        public string LocaleCode { get; set; }

        [DisplayName("Logo Image URL")]
        public string LogoImageURL { get; set; }

        [DisplayName("Cart Border Color")]
        public string CartBorderColor { get; set; }

        [DisplayName("Require Confirmed Shipping Address")]
        public bool RequireConfirmedShippingAddress { get; set; }

        [DisplayName("I have a PayPal Business Account")]
        public bool HaveBusinessAccount { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Signature { get; set; }

        public string SubjectEmailAddress { get; set; }
    }
}