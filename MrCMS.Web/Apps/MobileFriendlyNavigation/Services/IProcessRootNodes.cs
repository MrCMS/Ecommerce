using System.Collections.Generic;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public interface IProcessRootNodes
    {
        List<MobileFriendlyNavigationRootNode> Process(List<MobileFriendlyNavigationRootNode> nodes);
    }
}