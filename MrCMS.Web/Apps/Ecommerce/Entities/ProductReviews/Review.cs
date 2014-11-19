using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews
{
    public class Review : SiteEntity
    {
        [Required]
        public virtual int Rating { get; set; }
        [Required]
        public virtual string Email { get; set; }
        [Required]
        public virtual string Title { get; set; }
        [Required]
        public virtual string Text { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }

        public virtual User User { get; set; }

        public virtual bool? Approved { get; set; }
    }
}