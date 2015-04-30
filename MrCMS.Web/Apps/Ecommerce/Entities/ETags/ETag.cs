using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.ETags
{
    public class ETag : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
    }
}