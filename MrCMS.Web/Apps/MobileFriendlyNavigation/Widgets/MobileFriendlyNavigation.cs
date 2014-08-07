using System.Web.Mvc;
using MrCMS.Entities.Widget;
using NHibernate;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Widgets
{
    public class MobileFriendlyNavigation : Widget
    {
        public virtual bool IncludeChildren { get; set; }
    }
}