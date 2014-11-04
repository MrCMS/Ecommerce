using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Widgets
{
    public class MobileFriendlyNavigation : Widget
    {
        public virtual bool IncludeChildren { get; set; }

        public virtual Webpage RootWebpage { get; set; }
    }
}