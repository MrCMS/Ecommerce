using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities
{
    public class NewsletterTemplate : SiteEntity
    {
        public NewsletterTemplate()
        {
            Newsletters = new List<Newsletter>();
            ContentItemTemplateDatas = new List<ContentItemTemplateData>();
        }
        [Required]
        public virtual string Name { get; set; }
        [DisplayName("Base Template")]
        public virtual string BaseTemplate { get; set; }
        public virtual string Divider { get; set; }

        public virtual IList<Newsletter> Newsletters { get; set; }
        public virtual IList<ContentItemTemplateData> ContentItemTemplateDatas { get; set; }
    }
}