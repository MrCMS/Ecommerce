using System;
using System.ComponentModel;
using GCheckout;
using MrCMS.Settings;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class PaymentSettings : SiteSettingsBase
    {
        [DisplayName("Uses Paypal Express Checkout")]
        public bool UsesPaypalExpressCheckout { get; set; }

        [DisplayName("Paypal Express Checkout Url")]
        public string PaypalExpressCheckoutUrl { get; set; }

        [DisplayName("Uses Google Checkout")]
        public bool UsesGoogleCheckout { get; set; }

        [DisplayName("Google Checkout Merchant Id")]
        public string GoogleCheckoutMerchantID { get; set; }

        [DisplayName("Google Checkout Merchant Key")]
        public string GoogleCheckoutMerchantKey { get; set; }

        [DisplayName("Google Checkout Environment")]
        [DropDownSelection("google-checkout-environments")]
        public EnvironmentType GoogleCheckoutEnvironment { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }
        public override void SetViewData(NHibernate.ISession session, System.Web.Mvc.ViewDataDictionary viewDataDictionary)
        {
            viewDataDictionary["google-checkout-environments"] =
                Enum.GetValues(typeof (EnvironmentType))
                    .Cast<EnvironmentType>()
                    .BuildSelectItemList(type => type.ToString(), type => type.ToString(),
                                         type => type == GoogleCheckoutEnvironment, emptyItem: null);
        }
    }
}