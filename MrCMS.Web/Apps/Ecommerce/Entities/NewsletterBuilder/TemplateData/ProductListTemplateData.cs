using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.NewsletterBuilder.Attributes;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.TemplateData
{
    [TemplateDataFor(typeof(ProductList))]
    public class ProductListTemplateData : ContentItemTemplateData
    {
        [DisplayName("Product Grid Template")]
        public virtual string ProductGridTemplate { get; set; }
        [DisplayName("Product Row Template")]
        public virtual string ProductRowTemplate { get; set; }
        [DisplayName("Product Template")]
        public virtual string ProductTemplate { get; set; }
    }
}