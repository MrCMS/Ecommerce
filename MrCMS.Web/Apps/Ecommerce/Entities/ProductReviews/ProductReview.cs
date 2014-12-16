using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers.Validation;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews
{
    public class ProductReview : SiteEntity
    {
        [Required]
        public virtual int Rating { get; set; }

        [Required]
        public virtual string Name { get; set; }

        [Required]
        [EmailValidator]
        public virtual string Email { get; set; }

        [Required]
        [DisplayName("Review Title")]
        public virtual string Title { get; set; }

        [DisplayName("Review")]
        public virtual string Text { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }

        public virtual User User { get; set; }

        public virtual bool? Approved { get; set; }
    }
}