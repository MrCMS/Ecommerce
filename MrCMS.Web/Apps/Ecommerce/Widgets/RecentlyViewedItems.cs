using System.ComponentModel;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class RecentlyViewedItems : Widget
    {
        [DisplayName("Specify number of items for display")]
        public virtual int NoOfItemsForDisplay { get; set; }
    }
}