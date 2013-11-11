using MrCMS.Settings;
using SagePayMvc;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    public class SagePaySettings : SiteSettingsBase
    {
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

        public Configuration Configuration
        {
            get
            {
                return new Configuration
                           {
                               Mode = Mode,
                               VendorName = VendorName,
                           };
            }
        }

        public string VendorName { get; set; }

        public VspServerMode Mode { get; set; }
    }
}