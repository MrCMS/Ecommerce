using System.Linq;
using MrCMS.Entities.Widget;
using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class RecentlyViewedItems : Widget
    {
        [DisplayName("Specify number of items for display")]
        public virtual int NoOfItemsForDisplay { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            var noOfItemsForDisplay = NoOfItemsForDisplay > 0 ? NoOfItemsForDisplay : 5;
            var items = MrCMSApplication.Get<ITrackingService>().GetRecentlyViewedItems();
            if (items.Any() && items.Count >= noOfItemsForDisplay)
                items.Take(NoOfItemsForDisplay > 0 ? NoOfItemsForDisplay : 5);
            return items;
        }
    }
}
