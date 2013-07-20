using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Settings;
using PayPal.PayPalAPIInterfaceService.Model;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalExpressCheckoutSettings : SiteSettingsBase
    {
        public override bool RenderInSettings { get { return false; } }

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

        [DisplayName("User Name")]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Signature { get; set; }

        [DisplayName("Email Address")]
        public string SubjectEmailAddress { get; set; }

        public bool Enabled { get; set; }

        public CurrencyCodeType Currency { get; set; }
        public List<SelectListItem> CurrencyOptions
        {
            get
            {
                return Enum.GetValues(typeof (CurrencyCodeType)).Cast<CurrencyCodeType>()
                           .BuildSelectItemList(type => type.ToString(), selected: type => type == Currency,
                                                emptyItem: null);
            }
        }

        public PaymentActionCodeType PaymentAction { get; set; }
        public List<SelectListItem> PaymentActionOptions
        {
            get
            {
                return Enum.GetValues(typeof (PaymentActionCodeType)).Cast<PaymentActionCodeType>()
                           .BuildSelectItemList(type => type.ToString(), selected: type => type == PaymentAction,
                                                emptyItem: null);
            }
        }
    }
}