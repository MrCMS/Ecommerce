using System.Collections.Generic;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public interface IRootNodeProcessor
    {
        int Order { get; }
        List<MobileFriendlyNavigationRootNode> Process(List<MobileFriendlyNavigationRootNode> nodes);
    }
}