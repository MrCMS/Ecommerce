using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public interface IMobileFriendlyNavigationService
    {
        IEnumerable<MobileFriendlyNavigationRootNode> GetRootNodes(Widgets.MobileFriendlyNavigation widget);
        IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodes(Webpage parent);
    }
}