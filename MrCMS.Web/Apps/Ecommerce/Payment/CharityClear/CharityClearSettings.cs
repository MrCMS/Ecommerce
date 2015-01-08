using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CharityClear
{
    public class CharityClearSettings : SiteSettingsBase
    {
        public bool Enabled { get; set; }
        [DisplayName("CharityClear Merchant ID ")]
        public string MerchantId { get; set; }

        [DisplayName("Merchant Password (optional)")]
        public string MerchantPassword { get; set; }

        [DisplayName("Signature Key")]
        public string SignatureKey { get; set; }

        [DisplayName("ISO Country Code see http://en.wikipedia.org/wiki/ISO_3166-1_numeric")]
        public string ISOCountryCode { get; set; }

        [DisplayName("Display Name")]
        public string DisplayName { get; set; }
    }
}