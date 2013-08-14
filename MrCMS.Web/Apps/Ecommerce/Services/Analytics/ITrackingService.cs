using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics
{
    public interface ITrackingService
    {
        void AddItemToRecentlyViewedItemsCookie(int id);
        List<Product> GetRecentlyViewedItems();
    }
}