using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class BreadCrumb : Widget 
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var pages = CurrentRequestData.CurrentPage.ActivePages;

            return pages;
        }
    }
}