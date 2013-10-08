using System;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Amazon.Settings
{
    public class AmazonSyncSettings : SiteSettingsBase
    {
        public override bool RenderInSettings
        {
            get { return false; }
        }

        public DateTime? LastRun { get; set; }
    }
}
