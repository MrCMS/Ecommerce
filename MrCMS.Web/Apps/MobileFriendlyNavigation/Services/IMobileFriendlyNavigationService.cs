using System.Collections.Generic;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public interface IMobileFriendlyNavigationService
    {
        IEnumerable<MobileFriendlyNavigationRootNode> GetRootNodes();
        IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodes(int parentId);
    }
}