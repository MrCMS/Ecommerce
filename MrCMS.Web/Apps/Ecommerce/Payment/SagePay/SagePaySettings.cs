using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using MrCMS.Settings;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    public class SagePaySettings : SiteSettingsBase
    {
        public static CultureInfo CultureForTransactionEncoding = new CultureInfo("en-gb");
        public const string LiveUrl = "https://live.sagepay.com/gateway/service/vspserver-register.vsp";
        public const string TestUrl = "https://test.sagepay.com/gateway/service/vspserver-register.vsp";
        public const string SimulatorUrl = "https://test.sagepay.com/simulator/VSPServerGateway.asp?Service=VendorRegisterTx";

        public const string LiveRefundUrl = "https://live.sagepay.com/gateway/service/refund.vsp";
        public const string TestRefundUrl = "https://test.sagepay.com/gateway/service/refund.vsp";
        public const string SimulatorRefundUrl = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorRefundTx";

        public SagePaySettings()
        {
            Enabled = true;
            Mode = VspServerMode.Simulator;
            VendorName = "MrCMS Ecommerce";
        }
        public override bool RenderInSettings
        {
            get { return false; }
        }
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
                    case VspServerMode.Simulator:
                        return SimulatorUrl;
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
                    case VspServerMode.Simulator:
                        return SimulatorRefundUrl;
                    case VspServerMode.Test:
                        return TestRefundUrl;
                    case VspServerMode.Live:
                        return LiveRefundUrl;
                }

                return null;
            }
        }

        public bool RequiresSSL { get; set; }
    }
}