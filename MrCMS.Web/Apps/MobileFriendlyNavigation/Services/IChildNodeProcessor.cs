using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public interface IChildNodeProcessor
    {
        int Order { get; }
        List<MobileFriendlyNavigationChildNode> Process(List<MobileFriendlyNavigationChildNode> nodes, Webpage parent);
    }
}