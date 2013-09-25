using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Entities.Listings
{
    public class AmazonListingGroup: SiteEntity
    {
        public AmazonListingGroup()
        {
            Items=new List<AmazonListing>();
        }

        public virtual string Name { get; set; }

        public virtual IList<AmazonListing> Items { get; set; }

        public virtual bool IsListed
        {
            get
            {
                return Items != null && Items.Any(x => x.Status == AmazonListingStatus.Active);
            }
        }

        public virtual int NoOfItems
        {
            get { return Items.Count(); }
        }

        public virtual int NoOfActiveItems
        {
            get { return Items.Count(x => x.Status == AmazonListingStatus.Active); }
        }

        public virtual int NoOfInactiveItems
        {
            get { return Items.Count(x => x.Status == AmazonListingStatus.Inactive); }
        }

        public virtual int NoOfIdleItems
        {
            get { return Items.Count(x => x.Status == AmazonListingStatus.NotOnAmazon); }
        }
    }
}