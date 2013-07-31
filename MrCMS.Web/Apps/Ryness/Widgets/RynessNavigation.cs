using MrCMS.Entities.Widget;
using NHibernate;

namespace MrCMS.Web.Apps.Ryness.Widgets
{
    public class RynessNavigation : Widget
    {
        public override bool HasProperties
        {
            get { return false; }
        }

        public override object GetModel(ISession session)
        {
            return null;
        }

    }
}