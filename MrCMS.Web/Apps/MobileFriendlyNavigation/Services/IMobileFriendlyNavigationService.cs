using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public interface IMobileFriendlyNavigationService
    {
        List<MobileFriendlyNavigationRootNode> GetRootNodes(Webpage rootWebpage);
        List<MobileFriendlyNavigationChildNode> GetChildNodes(Webpage parent);
    }
}