using System.ComponentModel;
using MrCMS.Settings;
using MrCMS.Website.Caching;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class EcommerceSearchCacheSettings : SiteSettingsBase
    {
        [DisplayName("Cache search results?")]
        public virtual bool SearchCache { get; set; }

        [DisplayName("Cache results for how many seconds?")]
        public virtual int SearchCacheLength { get; set; }

        [DisplayName("Cache results expiry type")]
        public virtual CacheExpiryType SearchCacheExpiryType { get; set; }

        [DisplayName("Cache results per user (use if your site has wishlists/other per-user data in the results)")]
        public virtual bool SearchCachePerUser { get; set; }
    }
}