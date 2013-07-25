using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class RelatedProduct : Widget 
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var page = CurrentRequestData.CurrentPage;

            if (page is Product)
            {
                
            }
            return null;
        }
    }
}