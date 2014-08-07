using System.ComponentModel;
using MrCMS.Entities.Widget;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class CategoriesOnlyNavigation : Widget
    {
        [DisplayName("Max. number of levels for display in menu")]
        public virtual int NoOfMenuLevels { get; set; }

        public override bool HasProperties
        {
            get { return false; }
        }
    }
}
