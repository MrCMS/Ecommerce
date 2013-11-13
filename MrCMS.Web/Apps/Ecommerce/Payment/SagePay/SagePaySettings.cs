using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using MrCMS.Settings;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    public class SagePaySettings : SiteSettingsBase
    {
        public static CultureInfo CultureForTransactionEncoding = new CultureInfo("en-gb");
        public const string LiveUrl = "https://live.sagepay.com/gateway/service/vspserver-register.vsp";
        public const string TestUrl = "https://test.sagepay.com/gateway/service/vspserver-register.vsp";

        public const string LiveRefundUrl = "https://live.sagepay.com/gateway/service/refund.vsp";
        public const string TestRefundUrl = "https://test.sagepay.com/gateway/service/refund.vsp";

        public SagePaySettings()
        {
            Enabled = true;
            Mode = VspServerMode.Test;
            VendorName = "MrCMS Ecommerce";
            Protocol = "3.00";
            PaymentFormProfile = PaymentFormProfile.Low;
        }
        public override bool RenderInSettings
        {
            get { return false; }
        }
        public string Protocol { get; set; }
        public bool Enabled { get; set; }

        public string VendorName { get; set; }

        public VspServerMode Mode { get; set; }

        public IEnumerable<SelectListItem> ModeOptions
        {
            get
            {
                return Enum.GetValues(typeof(VspServerMode))
                           .Cast<VspServerMode>()
                           .BuildSelectItemList(mode => mode.ToString(), mode => mode.ToString(), mode => mode == Mode,
                                                emptyItem: null);
            }
        }


        /// <summary>
        /// The registration URL
        /// </summary>
        public string RegistrationUrl
        {
            get
            {
                switch (Mode)
                {
                    case VspServerMode.Test:
                        return TestUrl;
                    case VspServerMode.Live:
                        return LiveUrl;
                }
                return null;
            }
        }

        public string RefundUrl
        {
            get
            {
                switch (Mode)
                {
                    case VspServerMode.Test:
                        return TestRefundUrl;
                    case VspServerMode.Live:
                        return LiveRefundUrl;
                }

                return null;
            }
        }

        public bool RequiresSSL { get; set; }

        public PaymentFormProfile PaymentFormProfile { get; set; }
        private const string NormalFormMode = "NORMAL";
        private const string LowProfileFormMode = "LOW";
        public string PaymentFormProfileString
        {
            get
            {
                switch (PaymentFormProfile)
                {
                    case PaymentFormProfile.Low:
                        return LowProfileFormMode;
                    case PaymentFormProfile.Normal:
                        return NormalFormMode;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool AllowGiftAid { get; set; }

        public string AllowGiftAidString
        {
            get { return AllowGiftAid ? "1" : "0"; }
        }

        public ThreeDSecureSettings ThreeDSecureBehaviour { get; set; }
        public string Apply3DSecure { get { return ((int)ThreeDSecureBehaviour).ToString(); } }

        public AVSCV2Settings AVSCV2Behaviour { get; set; }
        public string ApplyAVSCV2 { get { return ((int)AVSCV2Behaviour).ToString(); } }
    }

    public enum AVSCV2Settings
    {
        Default = 0,
        ForceChecks = 1,
        IgnoreChecks = 2,
        ForceChecksButDoNotApply = 3,
    }

    public enum ThreeDSecureSettings
    {
        Default = 0,
        ForceChecks = 1,
        DoNotPerformChecks = 2,
        ForceButObtainAuthCode = 3
    }
}