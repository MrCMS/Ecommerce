using System.Collections.Generic;
using MrCMS.Services;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class MobileFriendlyNavigationStylesheets : IAppStylesheetList
    {
        public IEnumerable<string> UIStylesheets
        {
            get { yield return @"~/Apps/MobileFriendlyNavigation/Content/Styles/mobileFriendlyNavigation.css"; }
        }

        public IEnumerable<string> AdminStylesheets
        {
            get { yield break; }
        }
    }
}