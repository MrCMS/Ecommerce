using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Amazon.Entities.Listings
{
    public class AmazonListingItem: SiteEntity
    {
        public virtual AmazonListing AmazonListing { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }

        [DisplayName("Amazon Item ID")]
        public virtual string AmazonItemID { get; set; }

        [StringLength(80)]
        public virtual string Title { get; set; }

        public virtual int ConditionId { get; set; }

        public virtual AmazonListingItemStatus Status { get; set; }
    }
}