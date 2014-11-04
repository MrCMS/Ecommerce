using System.Collections.Generic;
using MrCMS.Services;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class MobileFriendlyNavigationScripts : IAppScriptList
    {
        public IEnumerable<string> UIScripts
        {
            get
            {
                yield return @"~/Apps/MobileFriendlyNavigation/Content/Scripts/mrcms/helpers.js";
                yield return @"~/Apps/MobileFriendlyNavigation/Content/Scripts/mrcms/mobileFriendlyNavigation.js";
            }
        }

        public IEnumerable<string> AdminScripts
        {
            get { yield break; }
        }
    }
}