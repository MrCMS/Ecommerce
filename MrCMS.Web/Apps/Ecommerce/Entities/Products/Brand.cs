using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class Brand : SiteEntity
    {
        [Remote("IsUniqueName", "Brand", AdditionalFields = "Id")]
        [StringLength(255)]
        [Required]
        public virtual string Name { get; set; }
        public virtual string Logo { get; set; }
    }
}