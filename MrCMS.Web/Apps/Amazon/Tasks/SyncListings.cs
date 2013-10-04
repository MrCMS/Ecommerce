using System;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;

namespace MrCMS.Web.Apps.Amazon.Tasks
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
