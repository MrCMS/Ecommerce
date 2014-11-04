using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder
{
    public class NewsletterTemplate : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }
        [DisplayName("Base Template")]
        public virtual string BaseTemplate { get; set; }
        public virtual string Divider { get; set; }
        [DisplayName("Free Text Template")]
        public virtual string FreeTextTemplate { get; set; }
        [DisplayName("Image Right and Text Left Template")]
        public virtual string ImageRightAndTextLeftTemplate { get; set; }
        [DisplayName("Image Left and Text Right Template")]
        public virtual string ImageLeftAndTextRightTemplate { get; set; }
        [DisplayName("Product Grid Template")]
        public virtual string ProductGridTemplate { get; set; }
        [DisplayName("Product Row Template")]
        public virtual string ProductRowTemplate { get; set; }
        [DisplayName("Product Template")]
        public virtual string ProductTemplate { get; set; }
        [DisplayName("Banner Template")]
        public virtual string BannerTemplate { get; set; }

        public virtual IList<Newsletter> Newsletters { get; set; }
    }
}