using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{

    [OutputCacheable(PerUser = true)]
    public class RecentlyViewedItems : Widget
    {
        [DisplayName("Specify number of items for display")]
        public virtual int NoOfItemsForDisplay { get; set; }
    }
}