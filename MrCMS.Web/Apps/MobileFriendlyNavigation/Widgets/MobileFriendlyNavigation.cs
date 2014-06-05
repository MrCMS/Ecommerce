using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Services;
using NHibernate;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Widgets
{
    public class MobileFriendlyNavigation : Widget
    {
        public virtual bool IncludeChildren { get; set; }

        public override object GetModel(ISession session)
        {
            var service = DependencyResolver.Current.GetService<IMobileFriendlyNavigationService>();
            return service.GetRootNodes();
        }
    }
}