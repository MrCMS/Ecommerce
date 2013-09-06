using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Amazon.Entities.Listings
{
    public class AmazonListing: SiteEntity
    {
        public AmazonListing()
        {
            Items=new List<AmazonListingItem>();
        }

        public virtual Product Product { get; set; }

        public virtual IList<AmazonListingItem> Items { get; set; }

        public virtual bool IsListed
        {
            get
            {
                return Items != null && Items.Any(x => x.Status == AmazonListingItemStatus.Listed);
            }
        }
    }
}