using System.Collections.Generic;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Stats.Services
{

    public class StatsUIScripts : IAppScriptList
    {
        public IEnumerable<string> UIScripts
        {
            get
            {
                yield return @"~/Apps/Stats/Content/Scripts/jquery.cookie.js";
                yield return @"~/Apps/Stats/Content/Scripts/analytics.js";
            }
        }

        public IEnumerable<string> AdminScripts
        {
            get { yield break; }
        }
    }
}