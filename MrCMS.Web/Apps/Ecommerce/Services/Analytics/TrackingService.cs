using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics
{
    public class TrackingService : ITrackingService
    {
        private const string RecentlyViewedItemsCookieName = "mrcms-recently-viewed-items";
        private readonly IProductService _productService;

        public TrackingService(IProductService productService)
        {
            _productService = productService;
        }

        public void AddItemToRecentlyViewedItemsCookie(int id)
        {
            var items=CookieHelper.GetValue(RecentlyViewedItemsCookieName);
            var newItem = !String.IsNullOrWhiteSpace(items)?items+","+id.ToString():id.ToString();
            CookieHelper.UpdateValue(RecentlyViewedItemsCookieName, newItem);
        }

        public List<Product> GetRecentlyViewedItems()
        {
            var value = CookieHelper.GetValue(RecentlyViewedItemsCookieName);
            var recentlyViewedItems = GetItemIds(value).Select(id => _productService.Get(id)).Where(product => product != null && product.Published).ToList();
            return recentlyViewedItems.Distinct().ToList();
        }

        private static IEnumerable<int> GetItemIds(string value)
        {
            var itemIds = new List<int>();
            try
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    var items = value.Split(',');
                    for (var i = items.Length - 1; i >= 0; i--)
                    {
                        if (String.IsNullOrWhiteSpace(items[i])) continue;

                        var productId = 0;
                        Int32.TryParse(items[i], out productId);
                        if (productId != 0)
                            itemIds.Add(productId);
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return itemIds;
        }
    }
}