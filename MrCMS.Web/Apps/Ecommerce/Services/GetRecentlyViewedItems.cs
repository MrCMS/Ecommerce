using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetRecentlyViewedItems:GetWidgetModelBase<RecentlyViewedItems>
    {
        private readonly ITrackingService _trackingService;

        public GetRecentlyViewedItems(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }

        public override object GetModel(RecentlyViewedItems widget)
        {
            var items = _trackingService.GetRecentlyViewedItems();
            var test = new List<Product>();
            if (items.Any())
            {
                var noOfItemsForDisplay = widget.NoOfItemsForDisplay;
                test.AddRange(items.Take(noOfItemsForDisplay > 0 ? noOfItemsForDisplay : 5));
                return test;
            }
            return items;
        }
    }
}