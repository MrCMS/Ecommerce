using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Settings
{
    public class WebExtensionsToRoute : SystemSettingsBase
    {
        public WebExtensionsToRoute()
        {
            Exentions = ".asp,.php,.aspx";
        }
        [DisplayName("Page extensions you want Mr CMS to handle")]
        [AppSettingName("file-extentions-to-handle")]
        public string Exentions { get; set; }

        public IEnumerable<string> Get => (Exentions ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}