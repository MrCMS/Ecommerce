using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    [OutputCacheable]
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