using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ryness.Entities
{
    public class BuyingGuideInformation : SiteEntity
    {
        [Required]
        public virtual string Title { get; set; }
        [Required]
        [AllowHtml]
        public virtual string Text { get; set; }
    }
}