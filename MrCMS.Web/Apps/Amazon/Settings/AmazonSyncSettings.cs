using System;
using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Amazon.Settings
{
    public class AmazonSyncSettings : SiteSettingsBase
    {
        public override bool RenderInSettings
        {
            get { return false; }
        }

        public bool TryCalculateVat { get; set; }

        public bool UseDefaultTaxRateForShippingTax { get; set; }

        [DisplayName("Sync last run")]
        public DateTime? LastRun { get; set; }
    }
}
