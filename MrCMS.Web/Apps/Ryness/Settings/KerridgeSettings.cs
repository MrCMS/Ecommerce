using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ryness.Settings
{
    public class KerridgeSettings : SiteSettingsBase
    {
        public bool Enabled { get; set; }
        public string KerridgeAccountNumber { get; set; }
        public string KerridgeBranch { get; set; }
        public string WebServiceUrl { get; set; }
        public string Id { get; set; }
        public string Password { get; set; }
    }
}