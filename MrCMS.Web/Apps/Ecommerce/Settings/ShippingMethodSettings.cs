using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class ShippingMethodSettings : SiteSettingsBase
    {
        public ShippingMethodSettings()
        {
            EnabledMethods = new Dictionary<string, bool>();
        }

        public Dictionary<string, bool> EnabledMethods { get; set; }

        public HashSet<Type> GetEnabledMethods()
        {
            return EnabledMethods.Keys.Where(s => EnabledMethods[s]).Select(TypeHelper.GetTypeByName).ToHashSet();
        }
    }
}