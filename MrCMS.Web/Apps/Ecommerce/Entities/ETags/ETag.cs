using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.ETags
{
    public class ETag : SiteEntity
    {
        [Required]
        [Remote("ValidateNameIsAllowed", "ETag", AdditionalFields = "Id")]
        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
    }
}