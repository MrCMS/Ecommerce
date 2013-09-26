using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Amazon.Entities.Listings
{
    public class AmazonListing: SiteEntity
    {
        public virtual AmazonListingGroup AmazonListingGroup { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }

        public virtual string ASIN { get; set; }

        public virtual AmazonListingStatus Status { get; set; }

        [Required]
        public virtual string Title { get; set; }
        [Required]
        [DisplayName("SKU")]
        public virtual string SellerSKU { get; set; }
        [Required]
        [DisplayName("Standard Product ID (UPC,EAN..)")]
        public virtual string StandardProductId { get; set; }
        [Required]
        [DisplayName("Standard Product ID Type")]
        public virtual StandardProductIDType StandardProductIDType { get; set; } 
        public virtual ConditionType Condition { get; set; }
        [DisplayName("Condition Note")]
        public virtual string ConditionNote { get; set; }
        [Required]
        public virtual int Quantity { get; set; }
        [Required]
        public virtual decimal Price { get; set; }
        [Required]
        public virtual string Currency { get; set; }
        [DisplayName("Release Date")]
        public virtual DateTime? ReleaseDate { get; set; }
        public virtual string Brand { get; set; }
        public virtual string Manafacturer { get; set; }
        [DisplayName("Manafacturer Part Number")]
        public virtual string MfrPartNumber { get; set; }
    }
}